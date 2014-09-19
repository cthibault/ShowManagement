using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool TryParseAsInt(this string input, out int result)
        {
            return TryParseAsInt(input, string.Empty, out result);
        }
        public static bool TryParseAsInt(this string input, string excludedCharacters, out int result)
        {
            if (excludedCharacters != null)
            {
                input = excludedCharacters.Aggregate(input, (current, c) => current.Replace(c.ToString(), string.Empty));
            }

            return int.TryParse(input, out result);
        }

        public static string SanitizeAsFilename(this string input)
        {
            return SanitizeAsFilename(input, string.Empty);
        }
        public static string SanitizeAsFilename(this string input, string replacementString)
        {
            if (input == null) return input;

            return Path.GetInvalidFileNameChars().Aggregate(input, (current, c) => current.Replace(c.ToString(), replacementString));
        }
    }
}
