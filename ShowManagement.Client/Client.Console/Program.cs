using Entities.Pattern;
using RestSharp;
using ShowManagement.Business.Enums;
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

            var client = new RestClient("http://localhost:25220");
            //var getAllRequest = new RestRequest("api/showInfo/", Method.GET);
            //var getAllResponse = await client.ExecuteGetTaskAsync<List<ShowInfo>>(getAllRequest);


            //var getOneRequest = new RestRequest("api/showInfo/", Method.GET);
            //getOneRequest.AddParameter("directoryPath", @"E:\Media\Videos\TV Shows\30 Rock");
            //var getOneResponse = await client.ExecuteGetTaskAsync<List<ShowInfo>>(getOneRequest);

            var req = new RestRequest("api/tvdb/searchSeries", Method.GET);
            req.AddParameter("seriesTitle", "Castle");
            var resp = await client.ExecuteGetTaskAsync<List<SeriesSearchResult>>(req);


            var getOneRequest = new RestRequest("api/tvdb/", Method.GET);
            getOneRequest.AddParameter("seriesId", 257655);
            getOneRequest.AddParameter("seasonNumber", 1);
            getOneRequest.AddParameter("episodeNumber", 1);
            var getOneResponse = await client.ExecuteGetTaskAsync<EpisodeData>(getOneRequest);


            var newShowInfo = new ShowInfo();
            newShowInfo.ObjectState = ObjectState.Added;
            newShowInfo.Name = "Client Console Test";
            newShowInfo.Directory = @"C:\Test Show\";
            newShowInfo.TvdbId = 0;
            newShowInfo.ImdbId = "imdbTest";
            newShowInfo.Parsers.Add(
                new Parser
                {
                    ObjectState = ObjectState.Added,
                    Type = ParserType.Season,
                    Pattern = "seasonPattern",
                    ExcludedCharacters = "seasonExclude"
                });
            newShowInfo.Parsers.Add(
                new Parser
                {
                    ObjectState = ObjectState.Added,
                    Type = ParserType.Episode,
                    Pattern = "episodePattern",
                    ExcludedCharacters = "episodeExclude"
                });

            var createOneRequest = new RestRequest("api/showInfo/", Method.POST);
            createOneRequest.RequestFormat = DataFormat.Json;
            createOneRequest.AddBody(newShowInfo);
            var createOneResponse = await client.ExecutePostTaskAsync<ShowInfo>(createOneRequest);


            var returnedShowInfo = createOneResponse.Data;

            returnedShowInfo.Name = "Client Console Test - Updated";
            returnedShowInfo.ObjectState = ObjectState.Modified;


            var updateRequest = new RestRequest("api/showInfo/", Method.POST);
            updateRequest.RequestFormat = DataFormat.Json;
            //updateRequest.AddParameter("id", returnedShowInfo.ShowId);
            updateRequest.AddBody(returnedShowInfo);
            var updateResponse = await client.ExecutePostTaskAsync<ShowInfo>(updateRequest);

            //var updateRequest = new RestRequest("api/showInfo/", Method.PUT);
            //updateRequest.RequestFormat = DataFormat.Json;
            ////updateRequest.AddParameter("id", returnedShowInfo.ShowId);
            //updateRequest.AddUrlSegment("id", returnedShowInfo.ShowId.ToString());
            //updateRequest.AddBody(returnedShowInfo);
            //var updateResponse = await client.ExecuteTaskAsync<ShowInfo>(updateRequest);


            var deleteRequest = new RestRequest("api/showInfo/", Method.DELETE);
            deleteRequest.RequestFormat = DataFormat.Json;
            deleteRequest.AddParameter("id", returnedShowInfo.ShowId);
            var deleteResponse = await client.ExecuteTaskAsync(deleteRequest); 
        }
    }
}
