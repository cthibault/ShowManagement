using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Core.Diagnostics
{
    public abstract class BaseTraceSourceManager
    {
        private static Dictionary<string, TraceSource> TraceSources = new Dictionary<string, TraceSource>();

        /// <summary>
        /// Try to find TraceSource instance based on key.  If one does not exist, create one. 
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>TraceSource instance with the provided source name</returns>
        public static TraceSource GetAddTraceSource(string name)
        {
            TraceSource traceSource = null;

            if (!TraceSources.TryGetValue(name, out traceSource))
            {
                traceSource = new TraceSource(name);

                TraceSources.Add(name, traceSource);
            }

            return traceSource;
        }
    }
}
