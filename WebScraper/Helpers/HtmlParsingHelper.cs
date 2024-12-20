using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WebScraper.Enums;
using TIAArticleAppAPI.Models;

namespace WebScraper.Helpers
{
    internal static class HtmlParsingHelper
    {
        internal static IEnumerable<HtmlNode> GetLinkElements(string bottomOfArticle)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(bottomOfArticle);
            IEnumerable<HtmlNode> linkElements = htmlDocument.DocumentNode.SelectNodes("//a");
            return linkElements;
        }
        internal static string? SplitHtmlBody(HtmlDocument htmlDoc)
        {
            string htmlBodyNode = htmlDoc.DocumentNode.InnerHtml;
            List<string> splitHtmlBody = htmlBodyNode.Split("alt=\"contact\">").ToList();
            if (splitHtmlBody.Count > 1)
            {
                List<string> cleanedHtmlBodyList = Regex.Split(splitHtmlBody[1], @"<!-- AddToAny BEGIN -->", RegexOptions.Singleline).ToList();
                if (cleanedHtmlBodyList.Count > 1)
                {
                    return cleanedHtmlBodyList[0];
                }
            }

            List<string> splitHtmlBodyOnSrc = htmlBodyNode.Split("src=\"images/A_contact.gif").ToList();
            if (splitHtmlBodyOnSrc.Count > 1)
            {
                List<string> cleanedHtmlBodyList = Regex.Split(splitHtmlBodyOnSrc[1], @"<!-- AddToAny BEGIN -->", RegexOptions.Singleline).ToList();
                if (cleanedHtmlBodyList.Count > 1)
                {
                    return cleanedHtmlBodyList[0];
                }
            }

            return null;
        }
        internal static HtmlNode ParseBody(HtmlNode node, string url)
        {
            FixLinks(node, url);
            FixImageUrls(node, url);
            return node;
        }

        internal static HtmlDocument LoadHtmlDocument(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }

        internal static void FixLinks(HtmlNode htmlBody, string url)
        {
            HtmlNodeCollection linkNodes = htmlBody.SelectNodes("//a[@href]");
        }
        internal static void FixImageUrls(HtmlNode htmlBody, string url)
        {
            HtmlNodeCollection imageNodes = htmlBody.SelectNodes("//img[@src]");
            if (imageNodes == null)
            {
                return;
            }
            foreach (HtmlNode imageNode in imageNodes)
            {
                string imageSrc = imageNode.GetAttributeValue("src", null);
                CleanLink(imageSrc, url, true);
            }
        }

        internal static string CleanLink(string link, string mainUrl, bool useTIADomain)
        {
            string domain = useTIADomain ? "https://traditioninaction.org" : "";

            if (link.Contains("../"))
            {
                link = domain + "/" + link.Replace("../", "");
            }
            else 
            {
                string category = GetCategoryFromURL(mainUrl);
                link = domain + "/" + category + "/" + link;
            }
            return link;
        }

        internal static string GetCategoryFromURL(string url)
        {
            string category = url.Split("/")[3];
            return category;
        }

        internal static string? ConvertStringToDate(string date)
        {
            string dateWithoutPosted = string.Empty;
            if (date.Contains("Posted on"))
            {
                dateWithoutPosted = date.Split("Posted on")[1].Trim().Replace("--", "01");
            }
            else
            {
                dateWithoutPosted = date.Split("Posted")[1].Trim().Replace("--", "01");
            }
            if (DateTime.TryParse(dateWithoutPosted, out DateTime parsedDate))
            {
                string formattedDate = parsedDate.ToString("yyyy-MM-dd");
                return formattedDate;
            }
            else
            {
                return null;
            }
        }
    }
}
