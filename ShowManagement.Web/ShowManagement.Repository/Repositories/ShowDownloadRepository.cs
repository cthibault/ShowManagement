using Repository.Pattern.Repository;
using ShowManagement.Business.Models;
using ShowManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ShowManagement.Repository.Queries;

namespace ShowManagement.Repository.Repositories
{
    public static class ShowDownloadRepository
    {
        public static List<ShowDownload> GetAll(this IRepositoryAsync<ShowDownload> repository, RangeParameter rangeParameter)
        {
            var downloadsByDateRange = new ShowDownloadByDateRangeQuery(rangeParameter);

            var query = repository
                .Query(downloadsByDateRange)
                .Select()
                .OrderByDescending(sd => sd.CreatedDate);

            var showDownloads = query.ToList();

            return showDownloads;
        }

        public static ShowDownload GetById(this IRepositoryAsync<ShowDownload> repository, int id)
        {
            var query = repository
                .Queryable()
                .Where(sd => sd.ShowDownloadId == id);

            var showDownload = query.SingleOrDefault();

            return showDownload;
        }

        public static ShowDownload GetByCurrentPath(this IRepositoryAsync<ShowDownload> repository, string currentPath)
        {
            var query = repository
                .Queryable()
                .Where(sd => sd.CurrentPath == currentPath);

            var showDownload = query.SingleOrDefault();

            return showDownload;
        }
    }
}
