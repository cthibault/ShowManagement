using ShowManagement.Business.Models;
using ShowManagement.CommonServiceProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.Services
{
    public class ServiceProvider : BaseServiceProvider, IServiceProvider
    {
        public ServiceProvider(string baseAddress)
            : base(baseAddress)
        {
        }

        public async Task<List<ShowInfo>> GetAllShows()
        {
            List<ShowInfo> showInfos = await this.GetAsync<List<ShowInfo>>("api/showInfo/Get");

            return showInfos;
        }

        public async Task<ShowInfo> GetShow(int showId)
        {
            var parameters = new Dictionary<string, object> { { "showId", showId } };

            ShowInfo showInfo = await this.GetAsync<ShowInfo>("api/showInfo/Get", parameters);

            return showInfo;
        }

        public async Task<ShowInfo> SaveShow(ShowInfo showInfo)
        {
            return showInfo;
        }
        public async Task<List<ShowInfo>> SaveShows(List<ShowInfo> showInfos)
        {
            return showInfos;
        }
    }
}