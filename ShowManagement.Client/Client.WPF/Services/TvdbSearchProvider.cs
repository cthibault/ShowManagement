
using Entities.Pattern;
using RestSharp;
using ShowManagement.Business.Models;
using ShowManagement.CommonServiceProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.Services
{
    public class TvdbSearchProvider : BaseServiceProvider, ITvdbSearchProvider
    {
        public TvdbSearchProvider(string baseAddress)
            : base(baseAddress)
        {
        }

        public async Task<List<SeriesSearchResult>> SearchForSeries(string seriesTitle)
        {
            List<SeriesSearchResult> results = null;

            var client = new RestClient(this.BaseAddress);

            var request = new RestRequest("api/tvdb/SearchForSeries", Method.GET);
            request.AddParameter("seriesTitle", seriesTitle);

            var response = await client.ExecuteGetTaskAsync<List<SeriesSearchResult>>(request);

            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                results = response.Data;
            }

            return results ?? new List<SeriesSearchResult>(0);
        }
    }
}