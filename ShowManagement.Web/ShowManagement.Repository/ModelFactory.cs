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

        public static ShowDownload Convert(ShowDownloadInfo showDownloadInfo)
        {
            ShowDownload showDownload = null;

            if (showDownload != null)
            {
                showDownload = new ShowDownload();
                showDownload.ObjectState = showDownloadInfo.ObjectState;
                showDownload.ShowDownloadId = showDownloadInfo.ShowDownloadId;
                showDownload.CurrentPath = showDownloadInfo.CurrentPath;
                showDownload.OriginalPath = showDownloadInfo.OriginalPath;
                showDownload.ModifiedDate = DateTime.Now;
                showDownload.CreatedDate = showDownloadInfo.CreatedDate ?? DateTime.Now;
            }

            return showDownload;
        }
        public static ShowDownloadInfo Convert(ShowDownload showDownload)
        {
            ShowDownloadInfo showDownloadInfo = null;

            if (showDownload != null)
            {
                showDownloadInfo = new ShowDownloadInfo();
                showDownloadInfo.ObjectState = showDownload.ObjectState;
                showDownloadInfo.ShowDownloadId = showDownload.ShowDownloadId;
                showDownloadInfo.CurrentPath = showDownload.CurrentPath;
                showDownloadInfo.OriginalPath = showDownload.OriginalPath;
                showDownloadInfo.ModifiedDate = showDownload.ModifiedDate;
                showDownloadInfo.CreatedDate = showDownload.CreatedDate;
            }

            return showDownloadInfo;
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
