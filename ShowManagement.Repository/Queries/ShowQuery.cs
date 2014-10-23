using Repository.Pattern.Ef6.Repository;
using ShowManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Repository.Queries
{
    public class ShowParserSimilarQuery : QueryObject<Show>
    {
        public string Pattern { get; set; }
        public string ExcludedCharacters { get; set; }

        public override System.Linq.Expressions.Expression<Func<Show, bool>> Query()
        {
            return s => s.ShowParsers.Any(sp => sp.Pattern == this.Pattern && sp.ExcludedCharacters == this.ExcludedCharacters);
        }
    }
}
