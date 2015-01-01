using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.Services
{
    public interface IRecentDownloadsServiceProvider
    {
        Task<List<ShowDownloadInfo>> GetRecentDownloads();
        Task<List<ShowDownloadInfo>> GetRecentDownloads(DateTime start);
    }
}