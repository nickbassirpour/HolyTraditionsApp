using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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

        internal static bool ContainsAnyOf(this string value, params string[] targets)
        {
            return targets.Any(target => target.Contains(value, StringComparison.OrdinalIgnoreCase));
        }

        internal static bool IsNullOrBadLink(this HtmlNode linkElement)
        {
            if (String.IsNullOrWhiteSpace(linkElement.InnerText)) return true;
            if (linkElement.InnerText.MatchesAnyOf(ScrapingHelper.linkTextsNotToScrape.ToArray())) return true;
            HtmlNodeCollection aTags = linkElement.SelectNodes(".//a");
            if (aTags == null || aTags.Count == 0 || aTags.Count > 1) return true;
            if (aTags.Any(a => String.IsNullOrWhiteSpace(a.InnerText))) return true;
            if (aTags.Any(a => a.GetAttributeValue("href", null).MatchesAnyOf(ScrapingHelper.linksNotToScrape.ToArray()))) return true;
            if (!aTags.Any(a => a.GetAttributeValue("href", null).Contains("."))) return true;
            return false;
        }

        internal static string[] linkTextsNotToScrape = new string[]
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
            "archives",
            "hot topics",
            "consequences"
        };

        internal static string[] linksNotToScrape = new string[]
        {
            "n000rpForgottenTruths.htm#forgotten"
        };

        internal static string[] linksWithNoDescription = new string[]
        {
            "our lady of good success",
            "the saint of the day",
        };

        internal static string[] categoriesWithNoSubcatregory = new string[]
        {
            "bev",
            "revolutionphotos",
            "progressivistdoc",
            "polemics",
            "bkreviews",
            "movies",
            "bestof",
            "olgs",
            "sod",
        };
    }
}
 