using Repository.Pattern.Repository;
using ShowManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Repository.Repositories
{
    public static class ShowRepository
    {
        public static IEnumerable<Show> GetShowsWithoutParsers(this IRepositoryAsync<Show> repository)
        {
            var query = repository
                .Queryable()
                .Where(s => !s.ShowParsers.Any());

            var result = query.AsEnumerable();

            return result;
        }
    }
}
