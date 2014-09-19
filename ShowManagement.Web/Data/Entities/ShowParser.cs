using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShowManagement.Web.Data.Entities
{
    public class ShowParser
    {
        public int ShowParserId { get; set; }
        public int Type { get; set; }
        public string Pattern { get; set; }
        public string ExcludedCharacters { get; set; }

        public int ShowId { get; set; }
        public virtual Show Show { get; set; }
    }
}