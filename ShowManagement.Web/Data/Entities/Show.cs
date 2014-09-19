using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShowManagement.Web.Data.Entities
{
    public class Show
    {
        public int ShowId { get; set; }
        public int TvdbId { get; set; }
        public string ImdbId { get; set; }
        public string Name { get; set; }
        public string Directory { get; set; }

        public virtual ICollection<ShowParser> ShowParsers { get; set; }
    }
}