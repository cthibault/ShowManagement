﻿using ShowManagement.Core.Extensions;
using ShowManagement.WindowsServices.NameResolver.Service.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Service
{
    [RunInstaller(true)]
    public class NameResolverWindowsServiceInstaller : Installer
    {
        public NameResolverWindowsServiceInstaller()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsServiceInstaller()");

            this._processInstaller = new ServiceProcessInstaller();
            this._processInstaller.Account = ServiceAccount.LocalSystem;
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Service Process Installer Account: {0}", this._processInstaller.Account);


            this._serviceInstaller = new ServiceInstaller();
            this._serviceInstaller.ServiceName = NameResolverWindowsService.SpecifiedServiceName;
            this._serviceInstaller.DisplayName = NameResolverWindowsService.SpecifiedServiceDisplayName;
            this._serviceInstaller.Description = string.Format("Version: {0}", NameResolverWindowsService.Version);
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Service Installer Service Name: {0}", this._serviceInstaller.ServiceName);

            this.Installers.Add(this._processInstaller);
            this.Installers.Add(this._serviceInstaller);

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Service.NameResolverWindowsServiceInstaller()");
        }

        private ServiceInstaller _serviceInstaller;
        private ServiceProcessInstaller _processInstaller;
    }
}
