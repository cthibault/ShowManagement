using ShowManagement.Repository.Repositories;
using Repository.Pattern.UnitOfWork;
using ShowManagement.Business.Models;
using ShowManagement.Entity;
using ShowManagement.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Entities.Pattern;
using ShowManagement.Repository;
using ShowManagement.Core.Extensions;

namespace ShowManagent.WebApi.Controllers
{
    public class ShowDownloadInfoController : ApiController
    {
        public ShowDownloadInfoController(IUnitOfWorkAsync uow)
        {
            this._uow = uow;
        }


        [HttpGet]
        public IEnumerable<ShowDownloadInfo> Get(DateTime start)
        {
            var showDownloadRepository = this._uow.RepositoryAsync<ShowDownload>();

            var rangeParameter = RangeParameter.Create(start);

            List<ShowDownload> showInfos = showDownloadRepository.GetAll(rangeParameter);

            List<ShowDownloadInfo> showDownloadInfos = showInfos.Select(sd => ModelFactory.Convert(sd)).ToList();

            return showDownloadInfos;
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response = null;

            try
            {
                var showDownloadRepository = this._uow.RepositoryAsync<ShowDownload>();

                ShowDownload showDownload = showDownloadRepository.GetById(id);

                if (showDownload != null)
                {
                    ShowDownloadInfo showDownloadInfo = ModelFactory.Convert(showDownload);

                    response = Request.CreateResponse(HttpStatusCode.OK, showDownloadInfo);
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            return response;
        }

        [HttpGet]
        public HttpResponseMessage Get(string currentPath)
        {
            HttpResponseMessage response = null;

            try
            {
                var showDownloadRepository = this._uow.RepositoryAsync<ShowDownload>();

                ShowDownload showDownload = showDownloadRepository.GetByCurrentPath(currentPath);

                if (showDownload != null)
                {
                    ShowDownloadInfo showDownloadInfo = ModelFactory.Convert(showDownload);

                    response = Request.CreateResponse(HttpStatusCode.OK, showDownloadInfo);
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            return response;
        }

        [HttpPost]
        public HttpResponseMessage Post(string currentPath, string newPath)
        {
            HttpResponseMessage response = null;

            try
            {
                if (!string.IsNullOrEmpty(currentPath) && !string.IsNullOrEmpty(newPath))
                {
                    var showDownloadRepository = this._uow.RepositoryAsync<ShowDownload>();

                    ShowDownload showDownload = showDownloadRepository.GetByCurrentPath(currentPath);

                    if (showDownload == null)
                    {
                        var now = DateTime.Now;

                        showDownload = new ShowDownload
                        {
                            ObjectState = ObjectState.Added,
                            CurrentPath = newPath,
                            OriginalPath = currentPath,
                            CreatedDate = now,
                            ModifiedDate =now,
                        };

                        showDownloadRepository.Insert(showDownload);
                    }
                    else
                    {
                        showDownload.ObjectState = ObjectState.Modified;
                        showDownload.CurrentPath = newPath;
                        showDownload.ModifiedDate = DateTime.Now;
                    }

                    this._uow.SaveChanges();

                    response = this.Get(showDownload.ShowDownloadId);
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Missing Current Path or New Path.");
                }
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            return response;
        }


        #region Private Fields
        private readonly IUnitOfWorkAsync _uow;
        #endregion
    }
}
