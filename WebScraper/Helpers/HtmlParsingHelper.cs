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
        internal static string SplitHtmlBody(HtmlDocument htmlDoc)
        {
            string htmlBodyNode = htmlDoc.DocumentNode.InnerHtml;
            string splitHtmlBody = htmlBodyNode.Split("alt=\"contact\">")[1];
            string cleanedHtmlBody = splitHtmlBody.Split("<!-- AddToAny BEGIN -->")[0];
            return cleanedHtmlBody;
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
            foreach (HtmlNode imageNode in imageNodes)
            {
                string imageSrc = imageNode.GetAttributeValue("src", null);
                if (imageSrc.Contains("../"))
                {
                    imageSrc = imageSrc.Replace("../", "/");
                } 
                else if (imageSrc.Contains("./"))
                {
                    imageSrc = imageSrc.Replace("./", "/");
                }

                if (imageSrc.Contains("shtml"))
                {
                    imageSrc = imageSrc.Replace("shtml", "html");
                } 
                else if (imageSrc.Contains("shtm"))
                {
                    imageSrc = imageSrc.Replace("shtm", "htm");
                }

                if (!imageSrc.Contains("/") && !imageSrc.Contains("http"))
                {
                    string category = GetCategoryFromURL(url);
                    imageSrc = "https://traditioninaction.org" + "/" + category + "/" + imageSrc;
                }
                else if (!imageSrc.Contains("http"))
                {
                    imageSrc = "https://traditioninaction.org" + imageSrc;
                }
            }
        }

        internal static string GetCategoryFromURL(string url)
        {
            string category = url.Split("/")[3];
            return category;
        }
    }
}
