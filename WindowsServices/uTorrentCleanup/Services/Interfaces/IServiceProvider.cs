using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.uTorrentCleanup.Services
{
    public interface IServiceProvider
    {
        Task SaveShowDownloadInfo(string showDownloadPath);
    }
}
