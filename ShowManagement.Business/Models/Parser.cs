using ShowManagement.Business.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShowManagement.Business.Models
{
    public class Parser
    {
        public Parser() { }

        public bool TryParse(string input, out string result)
        {
            bool success = false;

            if (!string.IsNullOrWhiteSpace(this.Pattern))
            {
                var regex = new Regex(this.Pattern);

                var match = regex.Match(input);

                success = match.Success;

                result = success ? match.Value : null;
            }
            else
            {
                success = true;

                result = input;
            }

            return success;
        }
        
        public int ParserId { get; set; }
        public ParserType Type { get; set; }
        public string Pattern { get; set; }
        public string ExcludedCharacters { get; set; }
    }
}
