using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TvdbLib;
using TvdbLib.Data;

namespace ShowManagent.WebApi.Controllers
{
    public class TvdbController : ApiController
    {
        public TvdbController()
        {

        }

        [HttpGet]
        public HttpResponseMessage Get(int seriesId, int seasonNumber, int episodeNumber)
        {
            HttpResponseMessage response = null;

            try
            {
                TvdbEpisode tvdbEpisode = this.TvdbHandler.GetEpisode(seriesId, seasonNumber, episodeNumber, TvdbEpisode.EpisodeOrdering.DefaultOrder, TvdbLanguage.DefaultLanguage);

                if (tvdbEpisode != null)
                {
                    EpisodeData episodeData = this.Convert(tvdbEpisode);

                    response = Request.CreateResponse(HttpStatusCode.OK, episodeData);
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return response;
        }

        [HttpGet]
        public HttpResponseMessage SearchForSeries(string seriesTitle)
        {
            HttpResponseMessage response = null;

            try
            {
                List<TvdbSearchResult> tvdbResults = this.TvdbHandler.SearchSeries(seriesTitle, TvdbLanguage.DefaultLanguage);

                if (tvdbResults != null)
                {
                    List<SeriesSearchResult> results = tvdbResults.Select(r => this.Convert(r)).ToList();

                    response = Request.CreateResponse(HttpStatusCode.OK, results);
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return response;
        }


        private EpisodeData Convert(TvdbEpisode tvdbEpisode)
        {
            EpisodeData episodeData = null;

            if (tvdbEpisode != null)
            {
                episodeData = new EpisodeData(
                    id: tvdbEpisode.Id,
                    imdbId: tvdbEpisode.ImdbId,
                    seriesId: tvdbEpisode.SeriesId,
                    seasonId: tvdbEpisode.SeasonId,
                    lastUpdated: tvdbEpisode.LastUpdated,
                    absoluteNumber: tvdbEpisode.AbsoluteNumber,
                    seasonNumber: tvdbEpisode.SeasonNumber,
                    episodeNumber: tvdbEpisode.EpisodeNumber,
                    episodeName: tvdbEpisode.EpisodeName,
                    overview: tvdbEpisode.Overview
                    );
            }

            return episodeData;
        }

        private SeriesSearchResult Convert(TvdbSearchResult tvdbSearchResult)
        {
            SeriesSearchResult searchResultData = null;

            if (tvdbSearchResult != null)
            {
                searchResultData = new SeriesSearchResult(
                    tvdbId: tvdbSearchResult.Id,
                    imdbId: tvdbSearchResult.ImdbId,
                    title: tvdbSearchResult.SeriesName,
                    overview: tvdbSearchResult.Overview,
                    firstAired: tvdbSearchResult.FirstAired
                );
            }

            return searchResultData;
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
