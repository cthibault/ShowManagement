using ShowManagement.Core.Extensions;
using ShowManagement.WindowsServices.NameResolver.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Components.Activities
{
    class ActivityComparer : IEqualityComparer<IActivity>
    {
        public bool Equals(IActivity x, IActivity y)
        {
            bool? isEquals = null;

            if (object.ReferenceEquals(x, y))
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "object.ReferenceEquals is TRUE.\r\n\t{0}\r\n\t{1}", x, y);
                isEquals = true;
            }
            else if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "One of the Activity objects is NULL.\r\n\t{0}\r\n\t{1}", x, y);
                isEquals = false;
            }
            else
            {
                isEquals = x.Equals(y);
            }

            if (!isEquals.HasValue)
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Warning, 0, "ShowManagement.WindowsServices.NameResolver.Components.Activities.ActivityComparer.Equals() failed to calculate Equality.");
                isEquals = false;
            }

            return isEquals.Value;
        }

        public int GetHashCode(IActivity obj)
        {
            if (object.ReferenceEquals(obj, null)) return 0;

            return obj.GetHashCode();
        }
    }
}
