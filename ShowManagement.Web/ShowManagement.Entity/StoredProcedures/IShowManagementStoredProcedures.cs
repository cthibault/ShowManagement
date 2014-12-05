using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Entity
{
    public interface IShowManagementStoredProcedures
    {
        IEnumerable<Show> Example(string directory);
    }
}
