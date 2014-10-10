using ShowManagement.Business.Enums;
using ShowManagement.Business.Models;
using ShowManagement.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TvdbLib.Data;

namespace ShowManagement.Web.Mappers
{
    public static class DtoMappers
    {
        public static ShowInfo ToShowInfo(Show show)
        {
            ShowInfo showInfo = null;

            if (show != null)
            {
                showInfo = new ShowInfo
                {
                    ShowId = show.ShowId,
                    TvdbId = show.TvdbId,
                    ImdbId = show.ImdbId,
                    Name = show.Name,
                    Directory = show.Directory,
                };

                showInfo.Parsers.AddRange(
                    show.ShowParsers.Select(sp => DtoMappers.ToParser(sp))
                                    .Where(sp => sp != null));
            }

            return showInfo;
        }
        public static Parser ToParser(ShowParser showParser)
        {
            Parser parser = null;

            if (showParser != null)
            {
                parser = new Parser
                {
                    ParserId = showParser.ShowParserId,
                    TypeKey = showParser.Type,
                    Pattern = showParser.Pattern,
                    ExcludedCharacters = showParser.ExcludedCharacters
                };
            }

            return parser;
        }

        public static Show ToShow(ShowInfo showInfo, bool forceInsert)
        {
            Show show = null;

            if (showInfo != null)
            {
                show = new Show
                {
                    ShowId = forceInsert ? 0 : showInfo.ShowId,
                    TvdbId = showInfo.TvdbId,
                    ImdbId = showInfo.ImdbId,
                    Name = showInfo.Name,
                    Directory = showInfo.Directory,
                };

                var showParsers = showInfo.Parsers != null
                    ? showInfo.Parsers.Select(p => DtoMappers.ToShowParser(p, forceInsert))
                                      .Where(p => p != null)
                                      .ToList()
                    : new List<ShowParser>();

                show.ShowParsers = new List<ShowParser>(showParsers);
            }

            return show;
        }
        public static ShowParser ToShowParser(Parser parser, bool forceInsert)
        {
            ShowParser showParser = null;

            if (parser != null)
            {
                showParser = new ShowParser
                {
                    ShowParserId = forceInsert ? 0 : parser.ParserId,
                    Type = parser.TypeKey,
                    Pattern = parser.Pattern,
                    ExcludedCharacters = parser.ExcludedCharacters
                };
            }

            return showParser;
        }

        public static EpisodeData ToEpisodeData(TvdbEpisode tvdbEpisode)
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
    }
}