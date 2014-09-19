using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.CommonServiceProviders
{
    public interface IShowManagementServiceProvider
    {
        Task<ShowInfo> GetShowInfo(string directoryPath);

        Task<EpisodeData> GetEpisodeData(int tvdbId, int seasonNumber, int episodeNumber);
    }
}
