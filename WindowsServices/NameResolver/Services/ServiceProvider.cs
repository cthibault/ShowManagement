using ShowManagement.Business.Models;
using ShowManagement.CommonServiceProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Services
{
    public class ServiceProvider : BaseServiceProvider, IServiceProvider
    {
        public ServiceProvider(string baseAddress)
            : base (baseAddress)
        {

        }

        public async Task<ShowInfo> GetShowInfo(string directoryPath)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "directoryPath", directoryPath },
            };

            ShowInfo showInfo = await this.GetAsync<ShowInfo>("api/showInfo/Get", parameters);

            return showInfo;
        }

        public async Task<EpisodeData> GetEpisodeData(int tvdbId, int seasonNumber, int episodeNumber)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "seriesId", tvdbId },
                { "seasonNumber", seasonNumber },
                { "episodeNumber", episodeNumber },
            };

            EpisodeData episodeData = await this.GetAsync<EpisodeData>("api/tvdb/GetEpisodeData", parameters);

            return episodeData;
        }
    }
}
