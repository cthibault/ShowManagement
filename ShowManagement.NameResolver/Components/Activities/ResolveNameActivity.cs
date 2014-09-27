using ShowManagement.Business.Enums;
using ShowManagement.Business.Models;
using ShowManagement.CommonServiceProviders;
using ShowManagement.Core.Extensions;
using ShowManagement.NameResolver.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Components.Activities
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
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Begin Performing Activity: ", this);

            IActivity nextActivity = null;

            FileInfo validFileInfo = this.FindAndValidateFileInfo();

            bool isSuccess = await this.PerformRename(validFileInfo);

            if (!isSuccess)
            {
                nextActivity = this.BuildNextActivity(isSuccess, validFileInfo != null);

                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Next Activity: ", nextActivity);
            }

            return nextActivity;
        }
        public override async Task Cancel()
        {
            this.IsCancelled = true;

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Cancelled Activity: ", this);
        }


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
            ResolveNameActivity nextActivity = null;

            if (!isSuccess && validFileInfoFound && this.MaxRetryAttempts > 0)
            {
                nextActivity = new ResolveNameActivity(this.FilePath, this.MaxRetryAttempts - 1, this.ServiceProvider);
                nextActivity.CachedEpisodeData = this.CachedEpisodeData;
            }

            return nextActivity;
        }
        

        private FileInfo FindAndValidateFileInfo()
        {
            var fileInfo = new FileInfo(this.FilePath);

            if (!fileInfo.Exists)
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "File Does Not Exist: {0}", fileInfo.FullName);
                fileInfo = null;
            }
            else if (fileInfo.Directory == null || !fileInfo.Directory.Exists)
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Season Directory Does Not Exist: {0}", fileInfo.Directory != null ? fileInfo.Directory.FullName : string.Empty);
                fileInfo = null;
            }
            else if (fileInfo.Directory.Parent == null || !fileInfo.Directory.Parent.Exists)
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Show Directory Does Not Exist: {0}", fileInfo.Directory.Parent != null ? fileInfo.Directory.Parent.FullName : string.Empty);
                fileInfo = null;
            }

            return fileInfo;
        }

        private async Task<bool> PerformRename(FileInfo fileInfo)
        {
            bool isSuccess = false;

            if (fileInfo != null)
            {
                if (this.CachedEpisodeData != null)
                {
                    isSuccess = await this.PerformRename(fileInfo, this.CachedEpisodeData);
                }
                else
                {
                    var showDirectoryFullName = fileInfo.Directory.Parent.FullName;

                    ShowInfo showInfo = await this.GetShowInfo(showDirectoryFullName);

                    if (this.IsValidToPerformRename(showInfo))
                    {
                        var parsedInfo = ParsedInfo.Parse(showInfo, fileInfo.Name);

                        TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Parse Successful: {0}, S:{1}, E:{2}", parsedInfo.IsParseSuccessful, parsedInfo.SeasonNumber, parsedInfo.EpisodeNumber);

                        if (parsedInfo.IsParseSuccessful)
                        {
                            var episodeData = await this.GetEpisodeData(parsedInfo);

                            if (episodeData != null)
                            {
                                this.CachedEpisodeData = episodeData;

                                isSuccess = await this.PerformRename(fileInfo, episodeData);
                            }
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

                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Renamed \"{0}\"  == TO ==  \"{1}\"", originalName, fullPath);
                }
                catch (IOException ioException)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught in ResolveNameActivity.PerformRename(); Unable to rename {0}  [{1}]", fileInfo.FullName, ioException.ExtractExceptionMessage());
                    isSuccess = false;
                }
                catch (Exception ex)
                {
                    // TODO
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught in ResolveNameActivity.PerformRename(): {0}", ex.ExtractExceptionMessage());
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        private bool IsValidToPerformRename(ShowInfo showInfo)
        {
            var isValid = showInfo != null
                && showInfo.TvdbId > 0
                && showInfo.Parsers.Any();

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Is Valid To Perform Rename: {0}", isValid);
            return isValid;
        }

        private async Task<ShowInfo> GetShowInfo(string showDirectoryFullName)
        {
            ShowInfo showInfo = null;

            try
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Calling GetShowInfo with Directory = {0}.", showDirectoryFullName);
                
                showInfo = await this.ServiceProvider.GetShowInfo(showDirectoryFullName);
            }
            catch (Exception ex)
            {
                // TODO
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught in ResolveNameActivity.GetShowInfo(): {0}", ex.ExtractExceptionMessage());
            }

            return showInfo;
        }

        private async Task<EpisodeData> GetEpisodeData(ParsedInfo parsedInfo)
        {
            EpisodeData episodeData = null;

            try
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Calling GetEpisodeData for {0} (Tvdb={1}) Season {2} Episode {3}",
                    parsedInfo.ShowInfo.Name, parsedInfo.ShowInfo.TvdbId, parsedInfo.SeasonNumber, parsedInfo.EpisodeNumber);

                episodeData = await this.ServiceProvider.GetEpisodeData(parsedInfo.ShowInfo.TvdbId, parsedInfo.SeasonNumber, parsedInfo.EpisodeNumber);
            }
            catch (Exception ex)
            {
                // TODO
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught in ResolveNameActivity.GetEpisodeData(): {0}", ex.ExtractExceptionMessage());
            }

            return episodeData;
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

            var fileName = string.Format("{0} - {1}", episodeData.EpisodeNumber, episodeData.EpisodeName);

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
            } while (File.Exists(fullpath));

            return fullpath;
        }


        public string FilePath { get; private set; }
        private IShowManagementServiceProvider ServiceProvider { get;   set; }
        private EpisodeData CachedEpisodeData { get; set; }


        private class ParsedInfo
        {
            public static ParsedInfo Parse(ShowInfo showInfo, string fileName)
            {
                if (showInfo == null)
                {
                    throw new ArgumentNullException("showInfo");
                }

                var parsedInfo = new ParsedInfo(showInfo, fileName);

                parsedInfo.Parse();

                return parsedInfo;
            }
            private ParsedInfo(ShowInfo showInfo, string fileName)
            {
                this.ShowInfo = showInfo;
            }

            private void Parse()
            {
                this.SeasonNumber = this.Parse(this.ShowInfo.Parsers.Where(p => p.Type == ParserType.Season), this.FileName);
                this.EpisodeNumber = this.Parse(this.ShowInfo.Parsers.Where(p => p.Type == ParserType.Episode), this.FileName);
            }

            private int Parse(IEnumerable<Parser> parsers, string fileName)
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ResolveNameActivity.Parse()");

                int parsedNumber = 0;

                if (parsers != null)
                {
                    foreach (var parser in parsers)
                    {
                        string result;
                        if (parser.TryParse(fileName, out result))
                        {
                            // TODO: Move this logic into the TryParse function
                            if (result.TryParseAsInt(parser.ExcludedCharacters, out parsedNumber))
                            {
                                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Parse was successful: {0}", parsedNumber);
                                break;
                            }
                        }
                    }
                }

                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ResolveNameActivity.Parse()");
                return parsedNumber;
            }

            public ShowInfo ShowInfo { get; private set; }
            public string FileName { get; private set; }

            public int SeasonNumber { get; private set; }
            public int EpisodeNumber { get; private set; }

            public bool IsParseSuccessful
            {
                get { return this.SeasonNumber > 0 && this.EpisodeNumber > 0; }
            }
        }
    }
}
