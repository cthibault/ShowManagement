using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Configuration.Install;
using ShowManagement.NameResolver.Services.Diagnostics;
using System.Diagnostics;

namespace ShowManagement.NameResolver.Services
{
    [RunInstaller(true)]
    public class NameResolverWindowsServiceInstaller : Installer
    {
        public NameResolverWindowsServiceInstaller()
        {
            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Enter ShowManagement.NameResolver.Services.NameResolverWindowsServiceInstaller()");

            this._processInstaller = new ServiceProcessInstaller();
            this._processInstaller.Account = ServiceAccount.LocalSystem;
            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Service Process Installer Account: {0}", this._processInstaller.Account);


            this._serviceInstaller = new ServiceInstaller();
            this._serviceInstaller.ServiceName = NameResolverWindowsService.SpecifiedServiceName;
            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Service Installer Service Name: {0}", this._serviceInstaller.ServiceName);

            this.Installers.Add(this._processInstaller);
            this.Installers.Add(this._serviceInstaller);

            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Exit ShowManagement.NameResolver.Services.NameResolverWindowsServiceInstaller()");
        }

        private ServiceInstaller _serviceInstaller;
        private ServiceProcessInstaller _processInstaller;
    }
}
