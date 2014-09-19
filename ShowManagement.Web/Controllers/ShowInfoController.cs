using ShowManagement.Business.Enums;
using ShowManagement.Business.Models;
using ShowManagement.Web.Converters;
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
            IEnumerable<Show> shows = this.UnitOfWork.ShowRepository.Get(null, null, "ShowParsers");

            var showInfos = shows.Select(s => DtoConverters.ToShowInfo(s)).ToList();

            return showInfos;
        }

        [HttpGet]
        [ResponseType(typeof(ShowInfo))]
        public IHttpActionResult Get(string directoryPath)
        {
            var show = this.UnitOfWork.ShowRepository
                .Get(s => s.Directory == directoryPath, null, "ShowParsers")
                .SingleOrDefault();

            if (show == null)
            {
                return NotFound();
            }

            var showInfo = DtoConverters.ToShowInfo(show);

            return this.Ok(showInfo);
        }

        //[HttpPost]
        //[ResponseType(typeof(ShowInfo))]
        //public IHttpActionResult PostShow(ShowInfo showInfo)
        //{
        //    var show = new Show();
        //    show.TvdbId = showInfo.TvdbId;
        //    show.ImdbId = showInfo.ImdbId;
        //    show.Name = showInfo.Name;
        //    show.Directory = showInfo.Directory;

        //    this.UnitOfWork.ShowRepository.Insert(show);

        //    this.UnitOfWork.Save();

        //    showInfo.ShowId = show.ShowId;

        //    return Ok(showInfo);
        //}
    }
}
