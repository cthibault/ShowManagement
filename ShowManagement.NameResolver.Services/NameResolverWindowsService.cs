using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using ShowManagement.Core.Extensions;
using ShowManagement.NameResolver.Components;
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

namespace ShowManagement.NameResolver.Services
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

            this.Start();
        }
        public void Start()
        {
            if (this.DirectoryMonitor != null)
            {
                this.DirectoryMonitor.Start();

                if (this.SettingsManager.InitialDirectoryScan)
                {
                    this.DirectoryMonitor.PerformFullScan();
                }
            }
        }

        protected override void OnStop()
        {
            this.Stop();

            base.OnStop();
        }
        public void Stop()
        {
            if (this.DirectoryMonitor != null)
            {
                this.DirectoryMonitor.Stop();
            }
        }

        private IUnityContainer UnityContainer
        {
            get
            {
                if (this._unityContainer == null)
                {
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
                    this._directoryMonitor = this.UnityContainer.Resolve<IDirectoryMonitor>(
                        new ParameterOverride("settingsManager", this.SettingsManager));
                }
                return this._directoryMonitor;
            }
        }
        private IDirectoryMonitor _directoryMonitor;


        public static void Install(string[] args)
        {
            Console.WriteLine("Installing...");

            try
            {
                if (IsServiceInstalled())
                {
                    NameResolverWindowsService.Uninstall(args);
                }

                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });

                Console.WriteLine("Install Complete");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Exception was raised during Service Install:");
                Console.Error.WriteLine(ex.ExtractExceptionMessage());
            }
        }

        public static void Uninstall(string[] args)
        {
            Console.WriteLine("Uninstalling...");

            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                Console.WriteLine("Uninstall Complete");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Exception was raised during Service Uninstall:");
                Console.Error.WriteLine(ex.ExtractExceptionMessage());
            }
        }

        private static bool IsServiceInstalled()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == NameResolverWindowsService.CustomServiceName);
        }

        public static string CustomServiceName = "SM.NameResolverWindowsService";
    }
}