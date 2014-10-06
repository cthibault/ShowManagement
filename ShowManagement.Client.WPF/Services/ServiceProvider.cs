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
            List<ShowInfo> showInfos = await this.GetAsync<List<ShowInfo>>("api/shows/GetShowInfos");

            return showInfos;
        }
    }
}