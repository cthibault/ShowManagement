using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ShowManagement.CommonServiceProviders
{
    public class ShowManagementServiceProvider : IShowManagementServiceProvider
    {
        public ShowManagementServiceProvider(string baseAddress)
        {
            this.BaseAddress = baseAddress;
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


        private async Task<T> GetAsync<T>(string apiUri, Dictionary<string, object> parameters)
        {
            T result = default(T);

            using (var client = new HttpClient())
            {
                var queryString = this.BuildQueryString(apiUri, parameters);

                client.BaseAddress = new Uri(this.BaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(queryString);

                response.EnsureSuccessStatusCode();

                result = await response.Content.ReadAsAsync<T>();
            }

            return result;
        }

        private string BuildQueryString(string apiUri, Dictionary<string, object> parameters)
        {
            string queryString = apiUri;

            var queryCollection = HttpUtility.ParseQueryString(string.Empty);

            if (parameters != null)
            {
                foreach (var parameterKvp in parameters)
                {
                    queryCollection[parameterKvp.Key] = parameterKvp.Value.ToString();
                }
            }

            var queryParameterString = queryCollection.ToString();

            if (!string.IsNullOrWhiteSpace(queryParameterString))
            {
                queryString += "?" + queryParameterString;
            }

            return queryString;
        }

        private string BaseAddress = string.Empty;
    }
}
