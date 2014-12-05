using ShowManagement.Business.Models;
using ShowManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Repository
{
    public static class ModelFactory
    {
        public static Show Convert(ShowInfo showInfo)
        {
            Show newShow = null;

            if (showInfo != null)
            {
                newShow = new Show();
                newShow.ObjectState = showInfo.ObjectState;
                newShow.ShowId = showInfo.ShowId;
                newShow.Name = showInfo.Name;
                newShow.Directory = showInfo.Directory;
                newShow.TvdbId = showInfo.TvdbId;
                newShow.ImdbId = showInfo.ImdbId;
            }

            return newShow;
        }
        public static ShowParser Convert(Parser parser, int showId)
        {
            ShowParser newParser = null;

            if (parser != null)
            {
                newParser = new ShowParser();
                newParser.ObjectState = parser.ObjectState;
                newParser.ShowParserId = parser.ParserId;
                newParser.Type = parser.TypeKey;
                newParser.Pattern = parser.Pattern;
                newParser.ExcludedCharacters = parser.ExcludedCharacters;
                newParser.ShowId = showId;
            }

            return newParser;
        }

        public static void Copy(Show from, Show to, bool includePrimaryKey)
        {
            if (from != null && to != null)
            {
                if (includePrimaryKey)
                {
                    to.ShowId = from.ShowId;
                }

                to.ObjectState = from.ObjectState;
                to.Name = from.Name;
                to.Directory = from.Directory;
                to.TvdbId = from.TvdbId;
                to.ImdbId = from.ImdbId;
            }
        }
        public static void Copy(ShowParser from, ShowParser to, bool includePrimaryKey)
        {
            if (from != null && to != null)
            {
                if (includePrimaryKey)
                {
                    to.ShowParserId = from.ShowParserId;
                }

                to.ObjectState = from.ObjectState;
                to.Type = from.Type;
                to.Pattern = from.Pattern;
                to.ExcludedCharacters = from.ExcludedCharacters;
                to.ShowId = from.ShowId;
            }
        }
    }
}
