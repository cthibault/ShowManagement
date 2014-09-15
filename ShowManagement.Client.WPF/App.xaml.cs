﻿using Microsoft.Practices.Unity;
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

            App.UnityContainer = unityContainer;

            base.OnStartup(e);
        }
        public static IUnityContainer UnityContainer { get; private set; }
    }
}