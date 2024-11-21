using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Helpers
{
    internal static class StringExtensions
    {
        public static bool MatchesAnyOf(this string value, params string[] targets)
        {
            return targets.Any(target => target.Equals(value, StringComparison.OrdinalIgnoreCase));
        }
    }
}
