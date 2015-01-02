using RestSharp;
using ShowManagement.Business.Models;
using ShowManagement.CommonServiceProviders;
using ShowManagement.WindowsServices.NameResolver.Exceptions;
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

        public async Task<List<ShowInfo>> GetShowInfos(string directoryPath)
        {
            List<ShowInfo> showInfos = null;

            var client = new RestClient(this.BaseAddress);

            var request = new RestRequest("api/showInfo/", Method.GET);
            request.AddParameter("directoryPath", directoryPath);

            var response = await client.ExecuteGetTaskAsync<List<ShowInfo>>(request);

            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                showInfos = response.Data;
            }

            return showInfos;
        }

        public async Task<EpisodeData> GetEpisodeData(int tvdbId, int seasonNumber, int episodeNumber)
        {
            EpisodeData episodeData = null;

            var client = new RestClient(this.BaseAddress);

            var request = new RestRequest("api/tvdb/", Method.GET);
            request.AddParameter("seriesId", tvdbId);
            request.AddParameter("seasonNumber", seasonNumber);
            request.AddParameter("episodeNumber", episodeNumber);

            var response = await client.ExecuteGetTaskAsync<EpisodeData>(request);

            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                episodeData = response.Data;
            }

            return episodeData;
        }

        public async Task SaveShowDownloadInfo(string originalShowPath, string newShowPath)
        {
            var client = new RestClient(this.BaseAddress);

            var request = new RestRequest("api/showDownloadInfo/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("currentPath", originalShowPath);
            request.AddParameter("newPath", newShowPath);

            var response = await client.ExecutePostTaskAsync<ShowDownloadInfo>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new RestServiceResponseException("Failed to Save", response);
            }
        }
    }
}
