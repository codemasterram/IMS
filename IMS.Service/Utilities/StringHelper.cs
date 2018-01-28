using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IMS.Logic.Utilities
{
    public static class StringHelper
    {
        public static string ReplaceString(this string output, string input, string replaceWith)
        {
            return Regex.Replace(output, input, replaceWith, RegexOptions.IgnoreCase);
        }
    }
}
