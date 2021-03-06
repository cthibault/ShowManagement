﻿using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using ShowManagement.WindowsServices.NameResolver.Components;
using ShowManagement.WindowsServices.NameResolver.UnitTest.Mocks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ShowManagement.WindowsServices.NameResolver.UnitTest
{
    public class NameResolverServiceTest
    {
        [Fact]
        public async void NotMonitoringOnCreate()
        {
            var unityContainer = new UnityContainer();
            //unityContainer.RegisterType<INameResolverService, NameResolverServiceMock>();
            //unityContainer.RegisterType<IDirectoryMonitor, DirectoryMonitor>();

            var settings = new SettingsManager(new Dictionary<string, string>
                {
                    { SettingsManager.ITEM_RETRY_ATTEMPTS_KEY, "2" },
                    { SettingsManager.ITEM_RETRY_DURATION_KEY, "1000" },
                });

            INameResolverEngine engine = new NameResolverEngine(settings, null);

            var t1 = engine.Add("Test One");
            var t2 = engine.Add("Test Two");
            var t3 = engine.Add("Test One");

            Task.WaitAll(t1, t2, t3);
        }

    }
}
