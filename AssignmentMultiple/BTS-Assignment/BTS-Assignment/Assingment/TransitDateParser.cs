using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BTS_Assignment
{
    public class TransitDateParser
    {
        const string c_datePattern = @"\s*(?<Year>\d+)-(?<Month>\d+)-(?<Day>\d+)\s*";

        public bool TryParse(string text, out DateTime date)
        {
            Regex dateRegex = new Regex(c_datePattern, RegexOptions.IgnoreCase);

            date = DateTime.Now;

            if (text == null)
                return false;

            if (dateRegex.IsMatch(text))
            {
                Match dateMatch = dateRegex.Match(text);

                int year, month, day = 0;

                if (!int.TryParse(dateMatch.Groups["Year"].Value, out year))
                {
                    return false;
                }

                if(!int.TryParse(dateMatch.Groups["Month"].Value, out month))
                {
                    return false;
                }

                if (!int.TryParse(dateMatch.Groups["Day"].Value, out day))
                {
                    return false;
                }

                date = new DateTime(year, month, day);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
