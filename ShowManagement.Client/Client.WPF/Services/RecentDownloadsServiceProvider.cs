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
    public class RecentDownloadsServiceProvider : BaseServiceProvider, IRecentDownloadsServiceProvider
    {
        public RecentDownloadsServiceProvider(string baseAddress)
            : base(baseAddress)
        {
        }


        public async Task<List<ShowDownloadInfo>> GetRecentDownloads()
        {
            DateTime start = ShowManagement.Core.Extensions.DateTimeExtensions.NullDate;

            List<ShowDownloadInfo> showDownloadInfos = await this.GetRecentDownloads(start);

            return showDownloadInfos;
        }
        public async Task<List<ShowDownloadInfo>> GetRecentDownloads(DateTime start)
        {
            List<ShowDownloadInfo> showDownloadInfos = null;

            var client = new RestClient(this.BaseAddress);

            var request = new RestRequest("api/showDownloadInfo/", Method.GET);
            request.AddParameter("start", start);

            var response = await client.ExecuteGetTaskAsync<List<ShowDownloadInfo>>(request);

            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                showDownloadInfos = response.Data;
            }

            return showDownloadInfos;
        }
    }
}