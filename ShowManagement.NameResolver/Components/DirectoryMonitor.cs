using Microsoft.Practices.Unity;
using ShowManagement.NameResolver.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Components
{
    public class DirectoryMonitor : BaseComponent, IDirectoryMonitor
    {
        public DirectoryMonitor(IUnityContainer unityContainer, SettingsManager settingsManager, INameResolverService nameResolverService)
            : base(unityContainer)
        {
            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            this.SettingsManager = settingsManager;
            this.NameResolverService = nameResolverService;
        }

        public void Start()
        {
            this.NameResolverService.Start();
            this.FileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            this.FileSystemWatcher.EnableRaisingEvents = false;
            this.NameResolverService.Stop();
        }

        public void PerformFullScan()
        {
            var searchOption = this.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var filePaths = Directory.EnumerateFiles(this.ParentDirectory, "*.*", searchOption);

            var fileInfos = filePaths.Select(fp => new FileInfo(fp));

            this.ProcessFileInfo(fileInfos, 1);
        }


        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            this.ProcessFileInfo(new FileInfo(fileSystemEventArgs.FullPath), this.ItemRetryAttempts);
        }

        private void ProcessFileInfo(FileInfo fileInfo, int retryAttempts)
        {
            if (fileInfo != null)
            {
                this.ProcessFileInfo(new List<FileInfo> { fileInfo }, retryAttempts);
            }
        }
        private void ProcessFileInfo(IEnumerable<FileInfo> fileInfos, int retryAttempts)
        {
            if (fileInfos != null)
            {
                var filteredFileInfos = fileInfos.Where(fi => this.IsSupportedFileType(fi) && this.IsEligibleToProcess(fi));

                var filteredFileNames = filteredFileInfos.Select(fi => fi.FullName);

                this.NameResolverService.Add(filteredFileNames, retryAttempts);
            }
        }

        private bool IsSupportedFileType(FileInfo fileInfo)
        {
            var supported = this.SupportedFileTypes.Any(type => type.Equals(fileInfo.Extension, StringComparison.CurrentCultureIgnoreCase));

            return supported;
        }
        private bool IsEligibleToProcess(FileInfo fileInfo)
        {
            var eligible = !char.IsDigit(fileInfo.FullName, 0);

            return eligible;
        }


        public bool IsMonitoring
        {
            get { return this.FileSystemWatcher.EnableRaisingEvents;}
        }
       
        protected INameResolverService NameResolverService
        {
            get
            {
                if (this._nameResolverService == null)
                {
                    this._nameResolverService = this.UnityContainer.Resolve<INameResolverService>();
                }
                return this._nameResolverService;
            }
            private set { this._nameResolverService = value; }
        }
        private INameResolverService _nameResolverService;


        private FileSystemWatcher FileSystemWatcher
        {
            get
            {
                if (this._fileSystemWatcher == null)
                {
                    this._fileSystemWatcher = new FileSystemWatcher(this.ParentDirectory);
                    this._fileSystemWatcher.EnableRaisingEvents = false;
                    this._fileSystemWatcher.IncludeSubdirectories = true;

                    this._fileSystemWatcher.Created += this.FileSystemWatcherOnCreated;
                }

                return this._fileSystemWatcher;
            }
        }
        private FileSystemWatcher _fileSystemWatcher;

        private string ParentDirectory
        {
            get { return this.SettingsManager.ParentDirectory; }
        }
        private bool IncludeSubdirectories
        {
            get { return this.SettingsManager.IncludeSubdirectories; }
        }
        private int ItemRetryAttempts
        {
            get
            {
                var retry = this.SettingsManager.ItemRetryAttempts;
                if (retry <= 0)
                {
                    retry = DEFAULT_ITEM_RETRY_ATTEMPTS;
                }
                return retry;
            }
        }
        private int ItemRetryDurationSeconds
        {
            get
            {
                var retry = this.SettingsManager.ItemRetryDurationSeconds;
                if (retry <= 0)
                {
                    retry = DEFAULT_ITEM_RETRY_DURATION_SECONDS;
                }
                return retry;
            }
        }
        private List<string> SupportedFileTypes
        {
            get
            {
                if (this._supportedFileTypes == null)
                {
                    this._supportedFileTypes = new List<string>(this.SettingsManager.SupportedFileTypes);
                }
                return this._supportedFileTypes;
            }
        }
        private List<string> _supportedFileTypes;

        private SettingsManager SettingsManager { get; set; }

        #region Constants
        public const int DEFAULT_ITEM_RETRY_ATTEMPTS = 15;
        public const int DEFAULT_ITEM_RETRY_DURATION_SECONDS = 300;    //5 minutes
        #endregion
    }
}
