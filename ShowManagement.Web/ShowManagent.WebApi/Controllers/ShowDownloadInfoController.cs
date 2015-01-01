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
            var showRepository = this._uow.RepositoryAsync<ShowDownload>();

            var rangeParameter = RangeParameter.Create(start);

            List<ShowDownload> showInfos = showRepository.GetAll(rangeParameter);

            List<ShowDownloadInfo> showDownloadInfos = showInfos.Select(sd => ModelFactory.Convert(sd)).ToList();

            return showDownloadInfos;
        }

        //// GET: api/ShowInfo/5
        //[HttpGet]
        //public HttpResponseMessage Get(int id)
        //{
        //    HttpResponseMessage response = null;

        //    try
        //    {
        //        var showRepository = this._uow.RepositoryAsync<Show>();

        //        ShowInfo showInfo = showRepository.GetShowInfo(id);

        //        if (showInfo != null)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.OK, showInfo);
        //        }
        //        else
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.NotFound);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
        //    }

        //    return response;
        //}

        //// GET: api/ShowInfo/directoryPath
        //[HttpGet]
        //public HttpResponseMessage Get(string directoryPath)
        //{
        //    HttpResponseMessage response = null;

        //    try
        //    {
        //        var showRepository = this._uow.RepositoryAsync<Show>();

        //        List<ShowInfo> showInfos = showRepository.GetShowInfos(directoryPath);

        //        if (showInfos != null)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.OK, showInfos);
        //        }
        //        else
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.NotFound);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
        //    }

        //    return response;
        //}


        //// POST: api/ShowInfo
        //[HttpPost]
        //public HttpResponseMessage Post([FromBody]ShowInfo showInfo)
        //{
        //    HttpResponseMessage response = null;

        //    try
        //    {
        //        if (showInfo != null)
        //        {
        //            Func<int> getShowId = () => showInfo.ShowId;

        //            var showRepository = this._uow.RepositoryAsync<Show>();

        //            if (showInfo.ObjectState == ObjectState.Added)
        //            {
        //                Show newShow = ModelFactory.Convert(showInfo);

        //                List<ShowParser> newParsers = showInfo.Parsers.Select(p => ModelFactory.Convert(p, 0)).ToList();

        //                newParsers.ForEach(sp => newShow.ShowParsers.Add(sp));

        //                showRepository.InsertOrUpdateGraph(newShow);

        //                getShowId = () => newShow.ShowId;
        //            }
        //            else 
        //            {
        //                if (showInfo.ObjectState == ObjectState.Modified)
        //                {
        //                    Show updatedShow = ModelFactory.Convert(showInfo);

        //                    Show existingShow = showRepository.GetShow(updatedShow.ShowId, false);

        //                    ModelFactory.Copy(updatedShow, existingShow, false);
        //                }

                        
        //                var changedParsers = showInfo.Parsers.Where(p => p.ObjectState != ObjectState.Unchanged);

        //                if (changedParsers.Any())
        //                {
        //                    var showParsersRepository = this._uow.RepositoryAsync<ShowParser>();

        //                    foreach (var parser in changedParsers)
        //                    {
        //                        switch (parser.ObjectState)
        //                        {
        //                            case ObjectState.Added:
        //                                ShowParser newParser = ModelFactory.Convert(parser, showInfo.ShowId);

        //                                showParsersRepository.Insert(newParser);
        //                                break;

        //                            case ObjectState.Modified:
        //                                ShowParser updatedParser = ModelFactory.Convert(parser, showInfo.ShowId);

        //                                ShowParser existingParser = showParsersRepository.Find(parser.ParserId);

        //                                ModelFactory.Copy(updatedParser, existingParser, false);
        //                                break;

        //                            case ObjectState.Deleted:
        //                                showParsersRepository.Delete(parser.ParserId);
        //                                break;
        //                        }
        //                    }
        //                }
        //            }

        //            this._uow.SaveChanges();

        //            int showId = getShowId();

        //            response = this.Get(showId);
        //        }
        //        else
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, "Could not read ShowInfo from the body.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
        //    }

        //    return response;
        //}


        //// DELETE: api/ShowInfo
        //[HttpDelete]
        //public HttpResponseMessage Delete(int id)
        //{
        //    HttpResponseMessage response = null;

        //    try
        //    {
        //        var showRepository = this._uow.RepositoryAsync<Show>();

        //        var existingShow = showRepository.GetShow(id, true);

        //        if (existingShow != null)
        //        {
        //            var showParserRepository = this._uow.RepositoryAsync<ShowParser>();

        //            for (int i = existingShow.ShowParsers.Count - 1; i >= 0; i--)
        //            {
        //                showParserRepository.Delete(existingShow.ShowParsers.ElementAt(i).ShowParserId);
        //            }
                    
        //            showRepository.Delete(existingShow.ShowId);
                    
        //            this._uow.SaveChanges();

        //            response = Request.CreateResponse(HttpStatusCode.OK);
        //        }
        //        else
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.NotFound);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
        //    }

        //    return response;
        //}


        #region Private Fields
        private readonly IUnitOfWorkAsync _uow;
        #endregion
    }
}
