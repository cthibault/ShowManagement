using Repository.Pattern.Ef6.Repository;
using ShowManagement.Business.Models;
using ShowManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Repository.Queries
{
    public class ShowDownloadByDateRangeQuery : QueryObject<ShowDownload>
    {
        public ShowDownloadByDateRangeQuery(RangeParameter rangeParameter)
        {
            if (rangeParameter == null)
            {
                throw new ArgumentNullException("rangeParamger");
            }

            this.RangeParameter = rangeParameter;
        }

        public RangeParameter RangeParameter { get; private set; }

        public override Expression<Func<ShowDownload, bool>> Query()
        {
            Expression<Func<ShowDownload, bool>> expression = null;

            if (this.RangeParameter.StartDate.HasValue && this.RangeParameter.EndDate.HasValue)
            {
                expression = sd => sd.CreatedDate >= this.RangeParameter.StartDate.Value && sd.CreatedDate <= this.RangeParameter.EndDate.Value;
            }
            else if (this.RangeParameter.StartDate.HasValue)
            {
                expression = sd => sd.CreatedDate >= this.RangeParameter.StartDate.Value;
            }
            else if (this.RangeParameter.EndDate.HasValue)
            {
                expression = sd => sd.CreatedDate <= this.RangeParameter.EndDate.Value;
            }
            else
            {
                expression = sd => true;
            }

            return expression;
        }
    }
}
