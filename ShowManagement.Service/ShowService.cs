using Repository.Pattern.Repository;
using Service.Pattern;
using ShowManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Service
{
    public class ShowService : Service<Show>, IShowService
    {
        public ShowService(IRepositoryAsync<Show> repository)
            : base(repository)
        {
        }
    }
}
