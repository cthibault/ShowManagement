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
    public class ServiceProvider : BaseServiceProvider, IServiceProvider
    {
        public ServiceProvider(string baseAddress)
            : base(baseAddress)
        {
        }

        public async Task<List<ShowInfo>> GetAllShows()
        {
            List<ShowInfo> showInfos = null;

            var client = new RestClient(this.BaseAddress);

            var request = new RestRequest("api/showInfo", Method.GET);

            var response = await client.ExecuteGetTaskAsync<List<ShowInfo>>(request);

            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                showInfos = response.Data;
            }

            return showInfos ?? new List<ShowInfo>();
        }

        public async Task<ShowInfo> GetShow(int showId)
        {
            ShowInfo showInfo = null;

            var client = new RestClient(this.BaseAddress);

            var request = new RestRequest("api/showInfo/{id}", Method.GET);
            request.AddParameter("id", showId);

            var response = await client.ExecuteGetTaskAsync<ShowInfo>(request);

            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                showInfo = response.Data;
            }

            return showInfo;
        }

        public async Task<ShowInfo> SaveShow(ShowInfo showInfo)
        {
            ShowInfo returnedShowInfo = null;

            var client = new RestClient(this.BaseAddress);

            var request = new RestRequest("api/showInfo/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(showInfo);

            var response = await client.ExecutePostTaskAsync<ShowInfo>(request);

            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                returnedShowInfo = response.Data;
            }
            else
            {
                string header = "Failed Service Call - SaveShow";

                throw new InvalidOperationException(string.Format("{0}\r\n{1}", header, ""));
            }

            return returnedShowInfo;
        }

        public async Task<List<ShowInfo>> SaveShows(List<ShowInfo> showInfos)
        {
            var returnedShowInfos = new List<ShowInfo>(showInfos.Count);

            var client = new RestClient(this.BaseAddress);

            foreach (var showInfo in showInfos)
            {
                if (showInfo.ObjectState == ObjectState.Deleted)
                {
                    var request = new RestRequest("api/showInfo/", Method.DELETE);
                    request.AddParameter("id", showInfo.ShowId);

                    var response = await client.ExecuteTaskAsync(request);

                    if (response.ResponseStatus != ResponseStatus.Completed)
                    {
                        string header = "Failed Service Call - SaveShow";

                        throw new InvalidOperationException(string.Format("{0}\r\n{1}", header, ""));
                    }
                }
                else
                {
                    var request = new RestRequest("api/showInfo/", Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddBody(showInfo);

                    var response = await client.ExecutePostTaskAsync<ShowInfo>(request);

                    if (response.ResponseStatus == ResponseStatus.Completed)
                    {
                        returnedShowInfos.Add(response.Data);
                    }
                    else
                    {
                        string header = "Failed Service Call - SaveShow";

                        throw new InvalidOperationException(string.Format("{0}\r\n{1}", header, ""));
                    }
                }
            }

            return returnedShowInfos;
        }
    }
}