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

        internal static void ParseBody(HtmlNode node)
        {
            FixLinks(node);
            FixImageUrls(node);
        }

        internal static void FixLinks(HtmlNode htmlBody)
        {
            HtmlNodeCollection linkNodes = htmlBody.SelectNodes("//a[@href]");
        }
        internal static void FixImageUrls(HtmlNode htmlBody)
        {
            HtmlNodeCollection imageNodes = htmlBody.SelectNodes("//img[@src]");
            foreach (HtmlNode imageNode in imageNodes)
            {
                string imageSrc = imageNode.GetAttributeValue("src", null);
                if (imageSrc.Contains("../"))
                {
                    imageSrc = imageSrc.Replace("../", "/");
                }
            }
        }
    }
}
