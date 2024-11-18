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
using WebScraper.Models;

namespace WebScraper.Helpers
{
    internal static class HtmlParsingHelper
    {
        internal static List<NodeModel> ParseListItems(HtmlNode footNotesNode)
        {
            var resultList = new List<NodeModel>();
            if (footNotesNode == null) return resultList;

            var liNodes = footNotesNode.Descendants("li");
            foreach (var liNode in liNodes)
            {
                List<NodeModel> nodeList = new List<NodeModel>();
                string liNodeText = liNode.InnerHtml;
                liNodeText.Replace("</li>", "").Replace("<li>", "");

                //ParseNode(liNodeText, resultList);
            }
            return resultList;
        }

        internal static void ParseBody(HtmlNode node, string url)
        {
            FixLinks(node, url);
            FixImageUrls(node, url);
            Console.WriteLine(node.InnerHtml);
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
