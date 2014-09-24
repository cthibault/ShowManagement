﻿using ShowManagement.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Services.Diagnostics
{
    internal sealed class TraceSourceManager : BaseTraceSourceManager
    {
        internal static TraceSource TraceSource
        {
            get
            {
                TraceSource traceSource = BaseTraceSourceManager.GetAddTraceSource(TraceSourceServicesKey);

                return traceSource;
            }
        }
        private static readonly string TraceSourceServicesKey = "ShowManagement.NameResolver.Services";
    }
}
