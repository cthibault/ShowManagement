using Entities.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Entity
{
    public partial class ShowParser : Entities.Pattern.Entity
    {
        public int ShowParserId { get; set; }
        public int Type { get; set; }
        public string Pattern { get; set; }
        public string ExcludedCharacters { get; set; }

        public int ShowId { get; set; }
        public virtual Show Show { get; set; }
    }
}
