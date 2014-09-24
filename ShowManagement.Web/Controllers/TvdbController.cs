using ShowManagement.Business.Models;
using ShowManagement.Web.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TvdbLib;
using TvdbLib.Data;

namespace ShowManagement.Web.Controllers
{
    public class TvdbController : ApiController
    {
        public TvdbController()
        {
            
        }

        //[HttpGet]
        //[ResponseType(typeof(ShowInfo))]
        //public IHttpActionResult SearchSeries()
        //{

        //    return NotFound();
        //}

        [HttpGet]
        [ResponseType(typeof(EpisodeData))]
        public IHttpActionResult GetEpisodeData(int seriesId, int seasonNumber, int episodeNumber)
        {
            EpisodeData episodeData = null;

            try
            {
                TvdbEpisode tvdbEpisode = this.TvdbHandler.GetEpisode(seriesId, seasonNumber, episodeNumber, TvdbEpisode.EpisodeOrdering.DefaultOrder, TvdbLanguage.DefaultLanguage);

                if (tvdbEpisode != null)
                {
                    episodeData = DtoConverters.ToEpisodeData(tvdbEpisode);
                }
            }
            catch (Exception ex)
            {
                // TODO
                throw;
            }
            
            if (episodeData == null)
            {
                return this.NotFound();
            }

            return this.Ok(episodeData);
        }

        private TvdbHandler TvdbHandler
        {
            get
            {
                if (this._tvdbHandler == null)
                {
                    string apiKey = System.Configuration.ConfigurationManager.AppSettings["Tvdb-ApiKey"];

                    this._tvdbHandler = new TvdbHandler(apiKey);
                }
                return this._tvdbHandler;
            }
        }
        private TvdbHandler _tvdbHandler;
    }
}
