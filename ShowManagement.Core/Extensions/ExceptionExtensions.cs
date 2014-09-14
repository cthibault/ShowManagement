using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ExtractExceptionMessage(this Exception ex)
        {
            var builder = new StringBuilder();

            BuildExceptionMessage(builder, ex);

            return builder.ToString();
        }

        private static void BuildExceptionMessage(StringBuilder builder, Exception ex)
        {
            if (builder != null && ex != null)
            {
                builder.AppendLine(ex.Message);
                BuildExceptionMessage(builder, ex.InnerException);
            }
        }
    }
}
