using Microsoft.Practices.Unity;
using ShowManagement.Core.Extensions;
using ShowManagement.WindowsServices.NameResolver.Components;
using ShowManagement.WindowsServices.NameResolver.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Components
{
    public class DirectoryMonitor : BaseComponent, IDirectoryMonitor
    {
        public DirectoryMonitor(IUnityContainer unityContainer, SettingsManager settingsManager, INameResolverEngine nameResolverEngine)
            : base(unityContainer)
        {
            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            this.SettingsManager = settingsManager;
            this.NameResolverEngine = nameResolverEngine;
        }

        public void Start()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Components.DirectoryMonitor.Start()");

            this.NameResolverEngine.Start();
            this.FileSystemWatcher.EnableRaisingEvents = true;
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Directory Monitor Started");

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Components.DirectoryMonitor.Start()");
        }

        public void Stop()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Components.DirectoryMonitor.Stop()");

            this.FileSystemWatcher.EnableRaisingEvents = false;
            this.NameResolverEngine.Stop();
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Directory Monitor Stopped");

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Components.DirectoryMonitor.Stop()");
        }

        public void PerformFullScan()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Components.DirectoryMonitor.PerformFullScan()");

            var searchOption = this.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Use Search Option {0} when enumerating files in directory = {1}", searchOption, this.ParentDirectory);

            var filePaths = Directory.EnumerateFiles(this.ParentDirectory, "*.*", searchOption);

            var fileInfos = filePaths.Select(fp => new FileInfo(fp));

            this.ProcessFileInfo(fileInfos, 1);

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Components.DirectoryMonitor.PerformFullScan()");
        }


        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Components.DirectoryMonitor.FileSystemWatcherOnCreated()");
            this.ProcessFileInfo(new FileInfo(fileSystemEventArgs.FullPath), this.ItemRetryAttempts);
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Components.DirectoryMonitor.FileSystemWatcherOnCreated()");
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
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Components.DirectoryMonitor.ProcessFileInfo()");

            if (fileInfos != null)
            {
                var filteredFileInfos = fileInfos.Where(fi => this.IsSupportedFileType(fi) && this.IsEligibleToProcess(fi));

                var filteredFileNames = filteredFileInfos.Select(fi => fi.FullName).ToList();

                if (filteredFileNames.Any())
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Passing {0} file paths into the NamResolverEngine", filteredFileNames.Count);
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "The file paths being processed are:\r\n\t", string.Join("\r\n\t", filteredFileNames));
                    this.NameResolverEngine.Add(filteredFileNames, retryAttempts);
                }
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Components.DirectoryMonitor.ProcessFileInfo()");
        }

        private bool IsSupportedFileType(FileInfo fileInfo)
        {
            var supported = this.SupportedFileTypes.Any(type => type.Equals(fileInfo.Extension, StringComparison.CurrentCultureIgnoreCase));

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Current File Path: {0}", fileInfo.FullName);
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Supported File Types: {0}", string.Join(", ", this.SupportedFileTypes));
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Is Current File supported: {0}", supported);

            return supported;
        }
        private bool IsEligibleToProcess(FileInfo fileInfo)
        {
            var eligible = !char.IsDigit(fileInfo.Name, 0);

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Current File Path: {0}", fileInfo.FullName);
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Is Current File eligible to process: {0}", eligible);

            return eligible;
        }


        public bool IsMonitoring
        {
            get { return this.FileSystemWatcher.EnableRaisingEvents;}
        }
       
        protected INameResolverEngine NameResolverEngine
        {
            get
            {
                if (this._nameResolverEngine == null)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Initializing the INameResolverEngine instance.");
                    this._nameResolverEngine = this.UnityContainer.Resolve<INameResolverEngine>();
                }
                return this._nameResolverEngine;
            }
            private set { this._nameResolverEngine = value; }
        }
        private INameResolverEngine _nameResolverEngine;


        private FileSystemWatcher FileSystemWatcher
        {
            get
            {
                if (this._fileSystemWatcher == null)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Initializing the FileSystemWatcher instance with Parent Directory: {0}.", this.ParentDirectory);
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
