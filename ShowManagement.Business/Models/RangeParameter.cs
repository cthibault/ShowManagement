using ShowManagement.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Business.Models
{
    public class RangeParameter
    {
        public static RangeParameter Create()
        {
            return new RangeParameter(null, null);
        }
        public static RangeParameter Create(DateTime start)
        {
            DateTime? startDate = start.IsNullDate() ? null : (DateTime?)start;

            return new RangeParameter(startDate, null);
        }
        public static RangeParameter Create(DateTime start, DateTime end)
        {
            DateTime? startDate = start.IsNullDate() ? null : (DateTime?)start;
            DateTime? endDate = end.IsNullDate() ? null : (DateTime?)end;

            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                throw new ArgumentOutOfRangeException("start", "Start Date is larger than End Date");
            }

            return new RangeParameter(startDate, endDate);
        }

        #region Constructors
        /// <summary>
        ///  For serialization purposes only
        /// </summary>
        public RangeParameter()
        {
        }

        private RangeParameter(DateTime? startDate, DateTime? endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
        } 
        #endregion

        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
    }
}
