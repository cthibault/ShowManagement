using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Core.Extensions
{
    public static class TraceSourceExtensions
    {
        public static void TraceWithDateFormat(this TraceSource source, TraceEventType eventType, int id, string message)
        {
            source.TraceEvent(eventType, id, string.Format("{0}\t{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), message));
        }

        public static void TraceWithDateFormat(this TraceSource source, TraceEventType eventType, int id, string format, params object[] args)
        {
            source.TraceEvent(eventType, id, string.Format("{0}\t{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), string.Format(format, args)));
        }
    }
}
