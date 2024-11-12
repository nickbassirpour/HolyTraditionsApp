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
            List<HtmlNode> nonEmptyNodes = SplitParagraphs(node);

            List<List<NodeModel>> articleNodes = new List<List<NodeModel>>();   
            foreach (HtmlNode nonEmptyNode in nonEmptyNodes)
            {
                articleNodes.Add(ParseParagraphNodes(nonEmptyNode.ChildNodes));
            }

            foreach (List<NodeModel> articleNode in articleNodes)
            {
                Console.WriteLine(JsonSerializer.Serialize(articleNode, new JsonSerializerOptions { WriteIndented = true }));
            }

        }

        private static List<HtmlNode> SplitParagraphs(HtmlNode node)
        {
            List<string> htmlParts = node.InnerHtml.Split("<br><br>").ToList();
            List<string> cleanedHtmlParts = new List<string>(); 
            foreach(string htmlPart in htmlParts) 
            {
                cleanedHtmlParts.Add(CleanText(htmlPart));
            };

            List<HtmlNode> nonEmptyHtmlNodes = RevertNodesToHtml(cleanedHtmlParts);

            return nonEmptyHtmlNodes;
        }

        private static string CleanText(string text)
        {
            string cleanedText = text.Replace("\n", " ").Replace("\t", " ");
            cleanedText = Regex.Replace(cleanedText, @"\s+", " ").Trim();
            return cleanedText;
        }

        private static List<HtmlNode> RevertNodesToHtml(List<string> nonEmptyNodes)
        {
            List<HtmlNode> nonEmptyHtmlNodes = new List<HtmlNode>();
            foreach (string nonEmptyNode in nonEmptyNodes)
            {
                HtmlDocument tempDoc = new HtmlDocument { };
                tempDoc.LoadHtml($"<div>{nonEmptyNode}</div>");
                HtmlNode nonEmptyHtmlNode = tempDoc.DocumentNode.FirstChild;
                nonEmptyHtmlNodes.Add(nonEmptyHtmlNode);
            }

            return nonEmptyHtmlNodes;
        }

        private static List<NodeModel> ParseParagraphNodes(HtmlNodeCollection childNodes)
        {
            List<NodeModel> articleNodes = new List<NodeModel>();
            foreach (HtmlNode childNode in childNodes)
            {
                if (childNode.Name == "img" || childNode.InnerHtml.Contains("<img"))
                {
                    articleNodes.Add(ParseImageNode(childNode));
                }
                if (childNode.Name == "a" || childNode.InnerHtml.Contains("<a"))
                {
                    articleNodes.Add(ParseLinkNode(childNode));
                }
                if (childNode.InnerHtml.Contains("<li>"))
                {
                    ParseLiElementNode(childNode);    
                }
                else
                {
                    articleNodes.Add(ParseTextNode(childNode));
                }
            }
            return articleNodes;
        }


        private static ImageNodeModel ParseImageNode(HtmlNode cleanedNode)
        {
            HtmlNode imgNode = cleanedNode.SelectSingleNode("//img[@src]");
            HtmlNode contentNode = cleanedNode.SelectSingleNode("//center") != null ? cleanedNode.SelectSingleNode("//center") : cleanedNode.SelectSingleNode("//p");

            return new ImageNodeModel
            {
                Type = NodeType.Image,
                ImageText = ParseParagraphNodes(contentNode.ChildNodes),
                Link = imgNode.GetAttributeValue("src", null),
                AltText = imgNode.GetAttributeValue("alt", null),
            };
        }

        private static NodeModel ParseLinkNode(HtmlNode cleanedNode)
        {
            (bool bold, bool italic, bool underline) = CheckStyling(cleanedNode);
            HtmlNode linkNode = cleanedNode.SelectSingleNode("//a");

            return new NodeModel
            {
                Type = NodeType.Link,
                Content = cleanedNode.InnerText.Trim(),
                Link = linkNode.GetAttributeValue("href", null),
                Italic = italic,
                Bold = bold,
                Underline = underline
            };
        }

        private static (bool, bool, bool) CheckStyling(HtmlNode node)
        {
            (bool bold, bool italic, bool underline) = (false, false, false);

            if (node.OuterHtml.Contains("<b>") || node.OuterHtml.Contains("<strong>")) bold = true;
            if (node.OuterHtml.Contains("<i>") || node.OuterHtml.Contains("<em>")) italic = true;
            if (node.OuterHtml.Contains("<u>")) underline = true;
            return (bold, italic, underline);
        }
        private static void ParseLiElementNode(HtmlNode childNode)
        {
            HtmlNode liNode = childNode.SelectSingleNode("//li");
            ParseParagraphNodes(liNode.ChildNodes);
        }

        private static NodeModel ParseTextNode(HtmlNode nonEmptyHtmlNode)
        {
            (bool bold, bool italic, bool underline) = CheckStyling(nonEmptyHtmlNode);

            return new NodeModel
            {
                Type = NodeType.Text,
                Content = nonEmptyHtmlNode.InnerText,
                Italic = italic,
                Bold = bold,
                Underline = underline
            };
        }
    }
}
