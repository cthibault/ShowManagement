using Microsoft.Practices.Unity;
using ShowManagement.Client.WPF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ShowManagement.Client.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var unityContainer = new UnityContainer();

            // Instance variables
            var settingsManager = new SettingsManager(ConfigurationManager.AppSettings);


            // Configuration
            unityContainer.RegisterInstance<SettingsManager>(settingsManager);

            //unityContainer.RegisterType<Services.IServiceProvider, Services.ServiceProvider>(new InjectionConstructor(settingsManager.BaseAddress));
            unityContainer.RegisterType<Services.IServiceProvider, Services.MockServiceProvider>();

            unityContainer.RegisterType<ViewModels.IShowsViewModel, ViewModels.ShowsViewModel>();


            App.UnityContainer = unityContainer;
            App.BusyContextManager = new BusyContextManager();

            base.OnStartup(e);
        }
        public static IUnityContainer UnityContainer { get; private set; }

        public static BusyContextManager BusyContextManager { get; private set; }
    }
}
