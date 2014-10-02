using ShowManagement.Core.Extensions;
using ShowManagement.WindowsServices.uTorrentCleanup.Service.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.uTorrentCleanup.Service
{
    [RunInstaller(true)]
    public partial class uTorrentCleanupWindowsServiceInstaller : Installer
    {
        public uTorrentCleanupWindowsServiceInstaller()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsServiceInstaller()");

            InitializeComponent();

            this._processInstaller = new ServiceProcessInstaller();
            this._processInstaller.Account = ServiceAccount.LocalSystem;
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Service Process Installer Account: {0}", this._processInstaller.Account);


            this._serviceInstaller = new ServiceInstaller();
            this._serviceInstaller.ServiceName = uTorrentCleanupWindowsService.SpecifiedServiceName;
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Service Installer Service Name: {0}", this._serviceInstaller.ServiceName);

            this.Installers.Add(this._processInstaller);
            this.Installers.Add(this._serviceInstaller);

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.uTorrentCleanup.Service.uTorrentCleanupWindowsServiceInstaller()");
        }

        private ServiceInstaller _serviceInstaller;
        private ServiceProcessInstaller _processInstaller;
    }
}
