using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("https://localhost:44300/");
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(
            //        new MediaTypeWithQualityHeaderValue("application/json"));

            //    HttpResponseMessage response = await client.GetAsync("api/shows/GetShowInfos");

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var content = await response.Content.ReadAsAsync<List<ShowInfo>>();
            //    }

            //    response = await client.GetAsync("api/shows/GetShowInfos2");

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var content = await response.Content.ReadAsAsync<List<ShowInfo>>();
            //    }
            //}

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44300/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var values = new Dictionary<string, string>()
                {
                    { "TvdbId", "0" },
                    { "ImdbId", null },
                    { "Name", "Test Data" },
                    { "Directory", @"C:\td\a" },
                };
                var content = new FormUrlEncodedContent(values);

                HttpResponseMessage response = await client.PostAsync("api/showInfo/PostShow", content);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<ShowInfo>();
                }
            }
        }
    }
}
