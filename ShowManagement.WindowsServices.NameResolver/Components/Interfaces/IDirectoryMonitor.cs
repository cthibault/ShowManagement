using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Components
{
    public interface IDirectoryMonitor
    {
        void Start();
        void Stop();
        void PerformFullScan();

        bool IsMonitoring { get; }
    }
}
