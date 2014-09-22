using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Configuration.Install;

namespace ShowManagement.NameResolver.Services
{
    [RunInstaller(true)]
    public class NameResolverWindowsServiceInstaller : Installer
    {
        public NameResolverWindowsServiceInstaller()
        {
            this._processInstaller = new ServiceProcessInstaller();
            this._processInstaller.Account = ServiceAccount.LocalSystem;

            this._serviceInstaller = new ServiceInstaller();
            this._serviceInstaller.ServiceName = NameResolverWindowsService.CustomServiceName;

            this.Installers.Add(this._processInstaller);
            this.Installers.Add(this._serviceInstaller);
        }

        private ServiceInstaller _serviceInstaller;
        private ServiceProcessInstaller _processInstaller;
    }
}
