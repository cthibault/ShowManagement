using ShowManagement.Business.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Business.Models
{
    public class Parser
    {
        public Parser(ParserType parserType, string pattern, string excludedCharacters)
        {
            this.ParserType = parserType;
            this.Pattern = pattern;
            this.ExcludedCharacters = excludedCharacters;
        }

        public bool TryParse(string input, out int value)
        {
            value = 0;

            return false;
        }
        
        public int ParserId { get; set; }
        public ParserType ParserType { get; set; }
        public string Pattern { get; set; }
        public string ExcludedCharacters { get; set; }
    }
}
