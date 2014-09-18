using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using ShowManagement.NameResolver.Components;
using ShowManagement.NameResolver.Services;
using ShowManagement.NameResolver.UnitTest.Mocks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ShowManagement.NameResolver.UnitTest
{
    public class DirectoryMonitorTests
    {
        [Fact]
        public void NotMonitoringOnCreate()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<INameResolverService, NameResolverServiceMock>();
            unityContainer.RegisterType<IDirectoryMonitor, DirectoryMonitor>();

            var settings = new SettingsManager(new Dictionary<string, string>
                {
                    { SettingsManager.PARENT_DIRECTORY_KEY, ConfigurationManager.AppSettings[SettingsManager.PARENT_DIRECTORY_KEY] },
                    { SettingsManager.ITEM_RETRY_ATTEMPTS_KEY, "2" },
                    { SettingsManager.ITEM_RETRY_DURATION_KEY, "1000" },
                });

            var directoryMonitor = unityContainer.Resolve<IDirectoryMonitor>(
                new ParameterOverride("settingsManager", settings));

            Assert.NotNull(directoryMonitor);
            Assert.False(directoryMonitor.IsMonitoring);
        }

        [Fact]
        public void MonitoringAfterStart()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<INameResolverService, NameResolverServiceMock>();
            unityContainer.RegisterType<IDirectoryMonitor, DirectoryMonitor>();

            var settings = new SettingsManager(new Dictionary<string, string>
                {
                    { SettingsManager.PARENT_DIRECTORY_KEY, ConfigurationManager.AppSettings[SettingsManager.PARENT_DIRECTORY_KEY] },
                    { SettingsManager.ITEM_RETRY_ATTEMPTS_KEY, "2" },
                    { SettingsManager.ITEM_RETRY_DURATION_KEY, "1000" },
                });

            var directoryMonitor = unityContainer.Resolve<IDirectoryMonitor>(
                new ParameterOverride("settingsManager", settings));

            Assert.NotNull(directoryMonitor);

            directoryMonitor.Start();

            Assert.True(directoryMonitor.IsMonitoring);
        }

        [Fact]
        public void NotMonitoringAfterStop()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<INameResolverService, NameResolverServiceMock>();
            unityContainer.RegisterType<IDirectoryMonitor, DirectoryMonitor>();

            var settings = new SettingsManager(new Dictionary<string, string>
                {
                    { SettingsManager.PARENT_DIRECTORY_KEY, ConfigurationManager.AppSettings[SettingsManager.PARENT_DIRECTORY_KEY] },
                    { SettingsManager.ITEM_RETRY_ATTEMPTS_KEY, "2" },
                    { SettingsManager.ITEM_RETRY_DURATION_KEY, "1000" },
                });

            var directoryMonitor = unityContainer.Resolve<IDirectoryMonitor>(
                new ParameterOverride("settingsManager", settings));

            Assert.NotNull(directoryMonitor);

            directoryMonitor.Start();
            directoryMonitor.Stop();

            Assert.False(directoryMonitor.IsMonitoring);
        }

        [Fact]
        public void PerformFullScan()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<INameResolverService, NameResolverServiceMock>();
            unityContainer.RegisterType<IDirectoryMonitor, DirectoryMonitor>();

            var settings = new SettingsManager(new Dictionary<string, string>
                {
                    { SettingsManager.PARENT_DIRECTORY_KEY, ConfigurationManager.AppSettings[SettingsManager.PARENT_DIRECTORY_KEY] },
                    { SettingsManager.INCLUDE_SUBDIRECTORIES_KEY, "true" },
                    { SettingsManager.SUPPORTED_FILE_TYPES_KEY, ".txt" },
                    { SettingsManager.ITEM_RETRY_ATTEMPTS_KEY, "2" },
                    { SettingsManager.ITEM_RETRY_DURATION_KEY, "1000" },
                });

            NameResolverServiceMock mockService = new NameResolverServiceMock();

            var directoryMonitor = unityContainer.Resolve<IDirectoryMonitor>(
                new ParameterOverride("settingsManager", settings),
                new ParameterOverride("nameResolverService", mockService));

            Assert.NotNull(directoryMonitor);

            directoryMonitor.PerformFullScan();

            Assert.Equal(3, mockService.FileNames.Count);
            Assert.Equal(1, mockService.RetryAttempts);
        }


        private IUnityContainer GetUnityContainerFromAppConfig()
        {
            var unityContainer = new UnityContainer();

            var configSection = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;

            Assert.NotNull(configSection);

            unityContainer.LoadConfiguration(configSection);

            return unityContainer;
        }
    }
}
