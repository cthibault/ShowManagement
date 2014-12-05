using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Pattern;

namespace ShowManagement.Entity
{
    public partial class Show : Entities.Pattern.Entity
    {
        public Show()
        {
            this.ShowParsers = new HashSet<ShowParser>();
        }

        public int ShowId { get; set; }
        public int TvdbId { get; set; }
        public string ImdbId { get; set; }
        public string Name { get; set; }
        public string Directory { get; set; }

        public virtual ICollection<ShowParser> ShowParsers { get; set; }
    }
}
