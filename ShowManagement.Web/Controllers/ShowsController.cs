using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ShowManagement.Web.Models;
using ShowManagement.Web.Data.Entities;
using ShowManagement.Web.Data.Repositories;
using ShowManagement.Business.Models;

namespace ShowManagement.Web.Controllers
{
    public class ShowsController : ApiController
    {
        public ShowsController()
        {

        }
        public ShowsController(IUnitOfWork unitOfWork)
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

        //private ApplicationDbContext db = new ApplicationDbContext();
        //// GET: api/Shows
        //public IQueryable<Show> GetShows()
        //{
        //    var query = db.Shows;

        //    return query;
        //}
        // GET: api/Shows
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



        //// GET: api/Shows/5
        //[ResponseType(typeof(Show))]
        //public IHttpActionResult GetShow(int id)
        //{
        //    Show show = db.Shows.Find(id);
        //    if (show == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(show);
        //}

        //// PUT: api/Shows/5
        //[Authorize(Roles = "canEdit")]
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutShow(int id, Show show)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != show.ShowId)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(show).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ShowExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/Shows
        //[Authorize(Roles = "canEdit")]
        //[ResponseType(typeof(Show))]
        //public IHttpActionResult PostShow(Show show)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Shows.Add(show);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = show.ShowId }, show);
        //}

        //// DELETE: api/Shows/5
        //[Authorize(Roles = "canEdit")]
        //[ResponseType(typeof(Show))]
        //public IHttpActionResult DeleteShow(int id)
        //{
        //    Show show = db.Shows.Find(id);
        //    if (show == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Shows.Remove(show);
        //    db.SaveChanges();

        //    return Ok(show);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._uow != null)
                {
                    this._uow.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        //private bool ShowExists(int id)
        //{
        //    return db.Shows.Count(e => e.ShowId == id) > 0;
        //}
    }
}