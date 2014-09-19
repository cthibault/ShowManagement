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
        public async Task<ShowInfo> GetShowInfo(string directoryPath)
        {
            ShowInfo showInfo = null;

            using (var client = new HttpClient())
            {
                var parameters = new Dictionary<string, string>()
                {
                    { "directoryPath", directoryPath }
                };

                var queryString = this.BuildQueryString("api/showInfo/Get", parameters);

                client.BaseAddress = new Uri(this.BaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(queryString);

                response.EnsureSuccessStatusCode();

                showInfo = await response.Content.ReadAsAsync<ShowInfo>();
            }

            return showInfo;
        }

        public async Task<EpisodeData> GetEpisodeData(int tvdbId, int seasonNumber, int episodeNumber)
        {
            // TODO: Implement

            return null;
        }


        private string BuildQueryString(string apiUri, Dictionary<string,string> parameters)
        {
            string queryString = apiUri;

            var queryCollection = HttpUtility.ParseQueryString(string.Empty);

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    queryCollection[parameter.Key] = parameter.Value;
                }
            }

            var queryParameterString = queryCollection.ToString();

            if (!string.IsNullOrWhiteSpace(queryParameterString))
            {
                queryString += "?" + queryParameterString;
            }

            return queryString;
        }

        private string BaseAddress = "https://localhost:44300/";
    }
}
