using ShowManagement.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Diagnostics
{
    internal sealed class TraceSourceManager : BaseTraceSourceManager
    {
        internal static TraceSource TraceSource
        {
            get
            {
                TraceSource traceSource = BaseTraceSourceManager.GetAddTraceSource(TraceSourceComponentsKey);

                return traceSource;
            }
        }
        private static readonly string TraceSourceComponentsKey = "ShowManagement.WindowsServices.NameResolver";
    }
}
