using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using ShowManagement.Core.Extensions;
using ShowManagement.WindowsServices.NameResolver.Components;
using ShowManagement.WindowsServices.NameResolver.Service.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Service
{
    partial class NameResolverWindowsService : ServiceBase
    {
        public NameResolverWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            this.TryStart();
        }
        public void TryStart()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsService.Start()");

            if (this.DirectoryMonitor != null)
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Start Directory Monitor");
                this.DirectoryMonitor.Start();

                if (this.SettingsManager.InitialDirectoryScan)
                {
                    this.DirectoryMonitor.PerformFullScan();
                }
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsService.Start()");
        }

        protected override void OnStop()
        {
            this.TryStop();

            base.OnStop();
        }
        public void TryStop()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsService.Stop()");

            if (this.DirectoryMonitor != null)
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Stop Directory Monitor");
                this.DirectoryMonitor.Stop();
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsService.Stop()");
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

        private IDirectoryMonitor DirectoryMonitor
        {
            get
            {
                if (this._directoryMonitor == null)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Initializing the IDirectoryMonitor instance.");

                    this._directoryMonitor = this.UnityContainer.Resolve<IDirectoryMonitor>(
                        new ParameterOverride("settingsManager", this.SettingsManager),
                        new ParameterOverride("baseAddress", this.SettingsManager.BaseAddress));
                }
                return this._directoryMonitor;
            }
        }
        private IDirectoryMonitor _directoryMonitor;


        public static void Install(string[] args)
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsService.Install()");
            Console.WriteLine("Installing...");

            try
            {
                if (IsServiceInstalled())
                {
                    NameResolverWindowsService.Uninstall(args);
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

                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught in ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsService.Install(): {0}", exceptionMessage);
                Console.Error.WriteLine("Exception was raised during Service Install:");
                Console.Error.WriteLine(exceptionMessage);
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsService.Install()");
        }

        public static void Uninstall(string[] args)
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsService.Uninstall()");
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

                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught in ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsService.Uninstall(): {0}", exceptionMessage);                
                Console.Error.WriteLine("Exception was raised during Service Uninstall:");
                Console.Error.WriteLine(exceptionMessage);
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsService.Uninstall()");
        }

        private static bool IsServiceInstalled()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Specified Service Name: {0}.", NameResolverWindowsService.SpecifiedServiceName);

            bool isServiceInstalled = ServiceController.GetServices().Any(s => s.ServiceName == NameResolverWindowsService.SpecifiedServiceName);
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Is Service Installed: {0}.", isServiceInstalled);

            return isServiceInstalled;
        }

        public static string SpecifiedServiceName = "SM.NameResolverWindowsService";
    }
}