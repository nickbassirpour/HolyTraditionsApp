using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Helpers
{
    internal static class ScrapingHelper
    {
        internal static bool MatchesAnyOf(this string value, params string[] targets)
        {
            return targets.Any(target => target.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        internal static bool IsNullOrBadLink(this HtmlNode linkElement)
        {
            if (String.IsNullOrWhiteSpace(linkElement.InnerText)) return true;
            if (linkElement.InnerText.MatchesAnyOf(ScrapingHelper.linksNotToScrape.ToArray())) return true;
            HtmlNodeCollection aTags = linkElement.SelectNodes(".//a");
            if (aTags.Any(a => String.IsNullOrWhiteSpace(a.InnerText))) return true;
            if (aTags == null || aTags.Count == 0 || aTags.Count > 1) return true;
            return false;
        }

        internal static string[] linksNotToScrape = new string[]
        {
            "home",
            "books",
            "cds",
            "search",
            "contact us",
            "donate",
            "forgotten truths",
            "religious",
            "news",
            "archives"
        };

        internal static string[] linksWithNoDescription = new string[]
        {
            "our lady of good success",
            "the saint of the day",
        };
    }
}
 