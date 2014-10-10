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

        public bool TryParse(string input, out int result)
        {
            bool success = false;

            result = 0;

            string parsedResult = null;

            if (this.TryParse(input, out parsedResult))
            {
                if (this.ExcludedCharacters != null)
                {
                    parsedResult = this.ExcludedCharacters.Aggregate(parsedResult, (current, c) => current.Replace(c.ToString(), string.Empty));
                }

                success = int.TryParse(parsedResult, out result);
            }

            return success;
        }
        private bool TryParse(string input, out string result)
        {
            bool success = false;

            if (input != null)
            {
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
            }
            else
            {
                success = false;

                result = null;
            }

            return success;
        }
        
        public int ParserId { get; set; }
        public ParserType Type
        {
            get { return (ParserType)this.TypeKey; }
            set { this.TypeKey = (int)value; }
        }
        public int TypeKey { get; set; }
        public string Pattern { get; set; }
        public string ExcludedCharacters { get; set; }
    }
}
