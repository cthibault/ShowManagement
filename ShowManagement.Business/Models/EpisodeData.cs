using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Business.Models
{
    public class EpisodeData
    {
        #region Constructors
        /// <summary>
        ///  For serialization purposes only
        /// </summary>
        public EpisodeData()
        {

        } 

        public EpisodeData(int id, string imdbId, int seriesId, int seasonId, DateTime lastUpdated, int absoluteNumber, int seasonNumber, int episodeNumber, string episodeName, string overview)
        {
            this.Id = id;
            this.ImdbId = imdbId;
            this.SeriesId = seriesId;
            this.SeasonId = SeasonId;
            this.LastUpdated = lastUpdated;
            this.AbsoluteNumber = absoluteNumber;
            this.SeasonNumber = SeasonNumber;
            this.EpisodeNumber = episodeNumber;
            this.EpisodeName = episodeName;
            this.Overview = overview;
        }
        #endregion

        public int Id { get; private set;}

        public string ImdbId { get; private set; }

        public int SeriesId { get; private set;}

        public int SeasonId { get; private set;}
        public DateTime LastUpdated { get; private set; }

        public int AbsoluteNumber { get; private set; }

        public int SeasonNumber { get; private set; }

        public int EpisodeNumber { get; private set; }

        public string EpisodeName { get; private set; }

        public string Overview { get; private set; }
    }
}
