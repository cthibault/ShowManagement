using ShowManagement.Business.Enums;
using ShowManagement.Business.Models;
using ShowManagement.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShowManagement.Web.Converters
{
    public static class DtoConverters
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

                showInfo.Parsers.AddRange(show.ShowParsers.Select(sp => ToParser(sp)));
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
                    Type = (ParserType)showParser.Type,
                    Pattern = showParser.Pattern,
                    ExcludedCharacters = showParser.ExcludedCharacters
                };
            }

            return parser;
        }
    }
}