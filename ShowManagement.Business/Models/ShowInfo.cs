using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Business.Models
{
    public class ShowInfo
    {
        public ShowInfo() { }

        public int ShowId { get; set; }
        public int TvdbId { get; set; }
        public string ImdbId { get; set; }
        public string Name { get; set; }
        public string Directory { get; set; }
        public List<Parser> Parsers
        {
            get
            {
                if (this._parsers == null)
                {
                    this._parsers = new List<Parser>();
                }
                return this._parsers;
            }
        }


        private List<Parser> _parsers;
    }
}
