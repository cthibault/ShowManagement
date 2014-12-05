using Repository.Pattern.Ef6.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Entity
{
    public partial class ShowManagementDataContext : IShowManagementStoredProcedures
    {
        public IEnumerable<Show> Example(string directory)
        {
            var directoryIdParameter = directory != null
                ? new SqlParameter("@Directory", directory)
                : new SqlParameter("@Directory", typeof(string));

            return this.Database.SqlQuery<Show>("Example @Directory");
        }
    }
}
