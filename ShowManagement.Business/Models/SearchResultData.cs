using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Business.Models
{
    public class SeriesSearchResult
    {
        #region Constructors
        /// <summary>
        ///  For serialization purposes only
        /// </summary>
        public SeriesSearchResult() { }
        public SeriesSearchResult(int tvdbId, string imdbId, string title, string overview, DateTime firstAired)
        {
            this.TvdbId = tvdbId;
            this.ImdbId = imdbId;
            this.Title = title;
            this.Overview = overview;
            this.FirstAired = firstAired;
        }
        #endregion

        #region Public Properties
        public int TvdbId { get; private set; }
        public string ImdbId { get; private set; }
        public string Title { get; private set; }
        public string Overview { get; private set; }
        public DateTime FirstAired { get; private set; }
        #endregion
    }
}
