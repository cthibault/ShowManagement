using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.Services
{
    public interface IServiceProvider
    {
        Task<List<ShowInfo>> GetAllShows();
        Task<ShowInfo> GetShow(int showId);

        Task<ShowInfo> SaveShow(ShowInfo showInfo);
        Task<List<ShowInfo>> SaveShows(List<ShowInfo> showInfos);
    }
}