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
    public abstract class BaseServiceProvider
    {
        protected BaseServiceProvider(string baseAddress)
        {
            this.BaseAddress = baseAddress;
        }

        public async Task<T> GetAsync<T>(string apiUri)
        {
            return await this.GetAsync<T>(apiUri, null);
        }
        
        public async Task<T> GetAsync<T>(string apiUri, Dictionary<string, object> parameters)
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

        public string BuildQueryString(string apiUri, Dictionary<string, object> parameters)
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

        protected string BaseAddress { get; private set; }
    }
}
