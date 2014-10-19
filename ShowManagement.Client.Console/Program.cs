using RestSharp;
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

            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("https://localhost:44300/");
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(
            //        new MediaTypeWithQualityHeaderValue("application/json"));

            //    var values = new Dictionary<string, string>()
            //    {
            //        { "TvdbId", "0" },
            //        { "ImdbId", null },
            //        { "Name", "Test Data" },
            //        { "Directory", @"C:\td\a" },
            //    };
            //    var content = new FormUrlEncodedContent(values);

            //    HttpResponseMessage response = await client.PostAsync("api/showInfo/PostShow", content);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var data = await response.Content.ReadAsAsync<ShowInfo>();
            //    }
            //}

            var client = new RestClient("https://localhost:44300");
            var request1 = new RestRequest("api/showInfo/Get/", Method.GET);
            var response1 = await client.ExecuteGetTaskAsync<List<ShowInfo>>(request1);


            var request2 = new RestRequest("api/showInfo/Get/{showId}", Method.GET);
            request2.AddParameter("showId", 1);
            var response2 = await client.ExecuteGetTaskAsync<ShowInfo>(request2);
            

            var show = response2.Data;
            
            var request3 = new RestRequest("api/showInfo/Post", Method.POST);
            request3.RequestFormat = DataFormat.Json;
            request3.AddBody(show);
            var response3 = await client.ExecutePostTaskAsync<ShowInfo>(request3);
        }
    }
}
