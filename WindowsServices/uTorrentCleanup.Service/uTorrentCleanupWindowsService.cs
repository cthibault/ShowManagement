using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using ShowManagement.Core.Extensions;
using ShowManagement.WindowsServices.uTorrentCleanup.Components;
using ShowManagement.WindowsServices.uTorrentCleanup.Service.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.uTorrentCleanup.Service
{
    partial class uTorrentCleanupWindowsService : ServiceBase
    {
        public uTorrentCleanupWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.OnStart()");

            base.OnStart(args);

            this.TryStart();

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.OnStart()");
        }
        public void TryStart()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.TryStart()");

            var interval = this.SettingsManager.CleanupIntervalInMinutes;

            var immediateTimer = Observable.Timer(TimeSpan.FromSeconds(1));
            var recurringTimer = Observable.Interval(TimeSpan.FromMinutes(interval));

            this._timer = Observable.Merge(immediateTimer, recurringTimer)
                .Subscribe(_ => this.TorrentManager.RemoveCompletedTorrents());

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.TryStart()");
        }

        protected override void OnStop()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.OnStop()");

            this.TryStop();

            base.OnStop();

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.OnStop()");
        }
        public void TryStop()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.TryStop()");

            if (this._timer != null)
            {
                this._timer.Dispose();
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.TryStop()");
        }



        private IUnityContainer UnityContainer
        {
            get
            {
                if (this._unityContainer == null)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Initializing the IUnityContainer instance.");
                    this._unityContainer = new UnityContainer();

                    var unityConfigSection = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;

                    this._unityContainer.LoadConfiguration(unityConfigSection);
                }
                return this._unityContainer;
            }
        }
        private IUnityContainer _unityContainer;

        private SettingsManager SettingsManager
        {
            get
            {
                if (this._settingsManager == null)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Initializing the SettingsManager instance.");
                    this._settingsManager = new SettingsManager(ConfigurationManager.AppSettings);
                }
                return this._settingsManager;
            }
        }
        private SettingsManager _settingsManager;

        private ITorrentManager TorrentManager
        {
            get
            {
                if (this._torrentManager == null)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Initializing the ITorrentManager instance.");

                    this._torrentManager = this.UnityContainer.Resolve<ITorrentManager>(
                        new ParameterOverride("settingsManager", this.SettingsManager));
                }
                return this._torrentManager;
            }
        }
        private ITorrentManager _torrentManager;

        private IDisposable _timer;



        public static void Install(string[] args)
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.Install()");
            Console.WriteLine("Installing...");

            try
            {
                if (IsServiceInstalled())
                {
                    uTorrentCleanupWindowsService.Uninstall(args);
                }

                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Assembly Location: {0}", assemblyLocation);


                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Initiate Install.");
                ManagedInstallerClass.InstallHelper(new string[] { assemblyLocation });
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Install Successful.");

                Console.WriteLine("Install Complete");
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.ExtractExceptionMessage();

                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught in ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.Install(): {0}", exceptionMessage);
                Console.Error.WriteLine("Exception was raised during Service Install:");
                Console.Error.WriteLine(exceptionMessage);
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.Install()");
        }

        public static void Uninstall(string[] args)
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.Uninstall()");
            Console.WriteLine("Uninstalling...");

            try
            {
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Assembly Location: {0}", assemblyLocation);


                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Initiate Uninstall.");
                ManagedInstallerClass.InstallHelper(new string[] { "/u", assemblyLocation });
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Uninstall Successful.");
                Console.WriteLine("Uninstall Complete");
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.ExtractExceptionMessage();

                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught in ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.Uninstall(): {0}", exceptionMessage);
                Console.Error.WriteLine("Exception was raised during Service Uninstall:");
                Console.Error.WriteLine(exceptionMessage);
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsService.Uninstall()");
        }

        private static bool IsServiceInstalled()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Specified Service Name: {0}.", uTorrentCleanupWindowsService.SpecifiedServiceName);

            bool isServiceInstalled = ServiceController.GetServices().Any(s => s.ServiceName == uTorrentCleanupWindowsService.SpecifiedServiceName);
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Is Service Installed: {0}.", isServiceInstalled);

            return isServiceInstalled;
        }

        public static string SpecifiedServiceName = "SM.uTorrentCleanupWindowsService";
    }
}
