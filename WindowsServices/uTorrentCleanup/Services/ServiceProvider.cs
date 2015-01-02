using RestSharp;
using ShowManagement.Business.Models;
using ShowManagement.CommonServiceProviders;
using ShowManagement.Core.Extensions;
using ShowManagement.WindowsServices.uTorrentCleanup.Diagnostics;
using ShowManagement.WindowsServices.uTorrentCleanup.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.uTorrentCleanup.Services
{
    public class ServiceProvider : BaseServiceProvider, Services.IServiceProvider
    {
        public ServiceProvider(string baseAddress)
            : base(baseAddress)
        {
        }

        public async Task SaveShowDownloadInfo(string showDownloadPath)
        {
            var client = new RestClient(this.BaseAddress);

            var request = new RestRequest("api/showDownloadInfo/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("currentPath", showDownloadPath);
            request.AddParameter("newPath", showDownloadPath);

            var response = await client.ExecutePostTaskAsync<ShowDownloadInfo>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new RestServiceResponseException("Failed to Save", response);
            }
        }
    }
}
