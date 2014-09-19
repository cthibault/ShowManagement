using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.Services
{
    class ServiceProvider
    {
        public async Task<List<ShowInfo>> GetAllShows()
        {
            List<ShowInfo> shows = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44300/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                await Task.Delay(5000);

                HttpResponseMessage response = await client.GetAsync("api/shows/GetShowInfos");

                response.EnsureSuccessStatusCode();

                shows = await response.Content.ReadAsAsync<List<ShowInfo>>();
            }

            return shows;
        }
    }
}
