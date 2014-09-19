using ShowManagement.Business.Enums;
using ShowManagement.Business.Models;
using ShowManagement.Web.Data.Entities;
using ShowManagement.Web.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ShowManagement.Web.Controllers
{
    public class ShowInfoController : ApiController
    {
        public ShowInfoController()
        {

        }
        public ShowInfoController(IUnitOfWork unitOfWork)
        {
            this._uow = unitOfWork;
        }

        #region UnitOfWork
        private IUnitOfWork _uow = null;
        protected IUnitOfWork UnitOfWork
        {
            get
            {
                if (this._uow == null)
                {
                    this._uow = new UnitOfWork();
                }
                return this._uow;
            }
        }
        #endregion

        [HttpGet]
        public IEnumerable<ShowInfo> GetShowInfos()
        {
            IEnumerable<Show> shows = this.UnitOfWork.ShowRepository.Get();

            var showInfos = shows.Select(s =>
                new ShowInfo()
                {
                    ShowId = s.ShowId,
                    TvdbId = s.TvdbId,
                    ImdbId = s.ImdbId,
                    Name = s.Name,
                    Directory = s.Directory,
                }).ToList();

            return showInfos;
        }

        [HttpGet]
        [ResponseType(typeof(ShowInfo))]
        public IHttpActionResult Get(string directoryPath)
        {
            var show = this.UnitOfWork.ShowRepository
                .Get(s => s.Directory == directoryPath)
                .SingleOrDefault();

            if (show == null)
            {
                return Ok(new ShowInfo());
            }

            var showInfo = new ShowInfo();
            showInfo.ShowId = show.ShowId;
            showInfo.TvdbId = show.TvdbId;
            showInfo.ImdbId = show.ImdbId;
            showInfo.Name = show.Name;
            showInfo.Directory = show.Directory;
            showInfo.Parsers.Add(new Parser(ParserType.Season, "season", "chars"));
            showInfo.Parsers.Add(new Parser(ParserType.Episode, "episode", "chars"));

            return this.Ok(showInfo);
        }

        [HttpPost]
        [ResponseType(typeof(ShowInfo))]
        public IHttpActionResult PostShow(ShowInfo showInfo)
        {
            var show = new Show();
            show.TvdbId = showInfo.TvdbId;
            show.ImdbId = showInfo.ImdbId;
            show.Name = showInfo.Name;
            show.Directory = showInfo.Directory;

            this.UnitOfWork.ShowRepository.Insert(show);

            this.UnitOfWork.Save();

            showInfo.ShowId = show.ShowId;

            return Ok(showInfo);
        }
    }
}
