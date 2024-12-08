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
using DataAccessLibrary.Models;

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
            if (splitHtmlBody.Count > 0)
            {
                string cleanedHtmlBody = Regex.Split(splitHtmlBody[0], @"<!-- AddToAny BEGIN -->", RegexOptions.Singleline)[0];
                return cleanedHtmlBody;
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

                //if (imageSrc.Contains("shtml"))
                //{
                //    imageSrc = imageSrc.Replace("shtml", "html");
                //} 
                //else if (imageSrc.Contains("shtm"))
                //{
                //    imageSrc = imageSrc.Replace("shtm", "htm");
                //}

                CleanLink(imageSrc, url, true);
            }
        }

        internal static string CleanLink(string link, string mainUrl, bool useTIADomain)
        {
            string linkWithCleanedSlashes = CleanSlashesFromLink(link);
            string linkWithCleanedSlashesAndDomain = AddDomainToLink(linkWithCleanedSlashes, mainUrl, useTIADomain);
            return linkWithCleanedSlashesAndDomain;
        }

        private static string CleanSlashesFromLink(string link)
        {
            if (link.Contains("../"))
            {
                link = link.Replace("../", "/");
            }
            else if (link.Contains("./"))
            {
                link = link.Replace("./", "/");
            }
            return link;
        }

        internal static string AddDomainToLink(string link, string mainUrl, bool useTIADomain)
        {
            string domain = useTIADomain ? "https://traditioninaction.org" : "";

            if (!link.Contains("/") && !link.Contains("http"))
            {
                string category = GetCategoryFromURL(mainUrl);
                link = domain + "/" + category + "/" + link;
            }
            else if (!link.Contains("http"))
            {
                link = domain + link;
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
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                string formattedDate = parsedDate.ToString("yyyy-MM-DD");
                return formattedDate;
            }
            else
            {
                return null;
            }
        }
    }
}
