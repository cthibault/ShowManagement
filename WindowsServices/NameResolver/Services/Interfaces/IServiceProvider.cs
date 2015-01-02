using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Services
{
    public interface IServiceProvider
    {
        Task<List<ShowInfo>> GetShowInfos(string directoryPath);

        Task<EpisodeData> GetEpisodeData(int tvdbId, int seasonNumber, int episodeNumber);

        Task SaveShowDownloadInfo(string originalShowPath, string newShowPath);
    }
}
