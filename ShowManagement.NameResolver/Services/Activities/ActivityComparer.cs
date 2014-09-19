using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Services.Activities
{
    class ActivityComparer : IEqualityComparer<IActivity>
    {
        public bool Equals(IActivity x, IActivity y)
        {
            if (object.ReferenceEquals(x, y)) return true;

            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;

            return x.Equals(y);
        }

        public int GetHashCode(IActivity obj)
        {
            if (object.ReferenceEquals(obj, null)) return 0;

            return obj.GetHashCode();
        }
    }
}
