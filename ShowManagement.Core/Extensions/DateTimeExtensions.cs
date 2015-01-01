using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime NullDate
        {
            get { return DateTime.MinValue; }
        }

        public static bool IsNullDate(this DateTime input)
        {
            return input.Equals(NullDate);
        }
    }
}
