using ShowManagement.Business.Enums;
using ShowManagement.Business.Models;
using ShowManagement.CommonServiceProviders;
using ShowManagement.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Services.Activities
{
    class ResolveNameActivity : Activity
    {
        public ResolveNameActivity(string filePath, int maxRetryAttempts, IShowManagementServiceProvider showManagementServiceProvider)
            : base(maxRetryAttempts)
        {
            this.FilePath = filePath;
            this.ServiceProvider = showManagementServiceProvider;
        }

        public override async Task<IActivity> Perform()
        {
            Trace.WriteLine("Performing Activity: " + this.ToString());

            IActivity nextActivity = null;

            FileInfo validFileInfo = this.FindAndValidateFileInfo();

            bool isSuccess = await this.PerformRename(validFileInfo);

            if (!isSuccess)
            {
                nextActivity = this.BuildNextActivity(isSuccess, validFileInfo != null);
            }

            return nextActivity;
        }
        public override async Task Cancel()
        {
            this.IsCancelled = true;

            Trace.WriteLine("Cancelled Activity: " + this.ToString());
        }

        public string FilePath { get; private set; }
        protected IShowManagementServiceProvider ServiceProvider { get; private set; }

        public override bool Equals(IActivity other)
        {
            bool isEquals = false;

            var activity = other as ResolveNameActivity;

            if (activity != null)
            {
                isEquals = this.FilePath.Equals(activity.FilePath);
            }

            return isEquals;
        }
        protected override int GetHashCodeImplementation()
        {
            return this.FilePath.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.FilePath, this.MaxRetryAttempts.ToString());
        }


        private IActivity BuildNextActivity(bool isSuccess, bool validFileInfoFound)
        {
            IActivity nextActivity = null;

            if (!isSuccess && validFileInfoFound && this.MaxRetryAttempts > 0)
            {
                nextActivity = new ResolveNameActivity(this.FilePath, this.MaxRetryAttempts - 1, this.ServiceProvider);
            }

            return nextActivity;
        }
        

        private FileInfo FindAndValidateFileInfo()
        {
            var fileInfo = new FileInfo(this.FilePath);

            if (!fileInfo.Exists)
            {
                Trace.WriteLine(string.Format("File Does Not Exist: {0}", fileInfo.FullName));
                fileInfo = null;
            }
            else if (fileInfo.Directory == null || !fileInfo.Directory.Exists)
            {
                Trace.WriteLine(string.Format("Season Directory Does Not Exist: {0}", fileInfo.Directory != null ? fileInfo.Directory.FullName : string.Empty));
                fileInfo = null;
            }
            else if (fileInfo.Directory.Parent == null || !fileInfo.Directory.Parent.Exists)
            {
                Trace.WriteLine(string.Format("Show Directory Does Not Exist: {0}", fileInfo.Directory.Parent != null ? fileInfo.Directory.Parent.FullName : string.Empty));
                fileInfo = null;
            }

            return fileInfo;
        }

        private async Task<bool> PerformRename(FileInfo fileInfo)
        {
            bool isSuccess = false;

            if (fileInfo != null)
            {
                var showDirectoryFullName = fileInfo.Directory.Parent.FullName;

                ShowInfo showInfo = await this.GetShowInfo(showDirectoryFullName);

                if (this.IsValidToPerformRename(showInfo))
                {
                    int seasonNumber = this.Parse(showInfo.Parsers.Where(p => p.Type == ParserType.Season), fileInfo.Name);
                    int episodeNumber = this.Parse(showInfo.Parsers.Where(p => p.Type == ParserType.Episode), fileInfo.Name);

                    Trace.WriteLine(string.Format("S:{0}, E:{1}", seasonNumber, episodeNumber));

                    if (seasonNumber > 0 && episodeNumber > 0)
                    {
                        var episodeData = await this.ServiceProvider.GetEpisodeData(showInfo.TvdbId, seasonNumber, episodeNumber);

                        if (episodeData != null)
                        {
                            isSuccess = await this.PerformRename(fileInfo, episodeData);
                        }
                    }
                }
            }

            return isSuccess;
        }
        private async Task<bool> PerformRename(FileInfo fileInfo, EpisodeData episodeData)
        {
            bool isSuccess = false;

            if (fileInfo != null && episodeData != null)
            {
                try
                {
                    var originalName = fileInfo.FullName;

                    var fullPath = this.BuildPath(fileInfo, episodeData);

                    fileInfo.MoveTo(fullPath);

                    isSuccess = true;

                    Trace.WriteLine(string.Format("Renamed \"{0}\"  == TO ==  \"{1}\"", originalName, fullPath));
                }
                catch (IOException ioException)
                {
                    Trace.WriteLine(string.Format("Unable to rename {0}  [{1}]", fileInfo.FullName, ioException.Message));
                    isSuccess = false;
                }
                catch (Exception ex)
                {
                    // TODO
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        private bool IsValidToPerformRename(ShowInfo showInfo)
        {
            return showInfo != null
                && showInfo.TvdbId > 0
                && showInfo.Parsers.Any();
        }

        private async Task<ShowInfo> GetShowInfo(string showDirectoryFullName)
        {
            ShowInfo showInfo = null;

            try
            {
                showInfo = await this.ServiceProvider.GetShowInfo(showDirectoryFullName);
            }
            catch (Exception ex)
            {
                // TODO
                Trace.WriteLine(ex.ExtractExceptionMessage());
            }

            return showInfo;
        }

        private int Parse(IEnumerable<Parser> parsers, string fileName)
        {
            int parsedNumber = 0;

            if (parsers != null)
            {
                foreach (var parser in parsers)
                {
                    string result;
                    if (parser.TryParse(fileName, out result))
                    {
                        if (result.TryParseAsInt(parser.ExcludedCharacters, out parsedNumber))
                        {
                            break;
                        }
                    }
                }
            }

            return parsedNumber;
        }

        private string BuildPath(FileInfo fileInfo, EpisodeData episodeData)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo");
            }

            if (episodeData == null)
            {
                throw new ArgumentNullException("episodeData");
            }

            var fileName = string.Format("{0} = {1}", episodeData.EpisodeNumber, episodeData.EpisodeName);

            var safeFilename = fileName.SanitizeAsFilename("_");

            string fullpath = string.Empty;
            int iteration = 0;

            do 
            {
                var copyValue = string.Empty;

                if (iteration > 0)
                {
                    copyValue = iteration == 1
                        ? " - Copy"
                        : string.Format(" - Copy ({0})", iteration);
                }

                fullpath = Path.Combine(fileInfo.DirectoryName, safeFilename + copyValue + fileInfo.Extension);

                iteration++;
            } while (File.Exists(fullpath) || iteration < 50);

            return fullpath;
        }
    }
}
