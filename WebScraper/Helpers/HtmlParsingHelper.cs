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

                ParseNode(liNodeText, resultList);
            }
            return resultList;
        }

        internal static void ParseBody(HtmlNode node)
        {
            List<HtmlNode> cleanedNodes = new List<HtmlNode>();
            var nodes = node.ChildNodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (!string.IsNullOrEmpty(nodes[i].InnerText.Trim())) cleanedNodes.Add(nodes[i]);
            }

            List<NodeModel> articleNodes = new List<NodeModel>();   
            foreach (HtmlNode cleanedNode in cleanedNodes)
            {
                bool italic = false;
                bool bold = false;
                bool underline = false;
                NodeType type = NodeType.Text;

                string? src = "";
                string altText = "";
                NodeModel caption = null;

                if (cleanedNode.Name == "img" || cleanedNode.InnerHtml.Contains("<img"))
                {
                    type = NodeType.Image;
                    src = cleanedNode.SelectSingleNode("//img/@src").ToString();
                    altText = cleanedNode.GetAttributeValue("alt", "");
                    caption = (new NodeModel
                    {
                        Content = cleanedNode.InnerText
                    });

                    articleNodes.Add(new ImageNodeModel
                    {
                        Type = type,
                        Content = cleanedNode.InnerText,
                        Src = src,
                        AltText = altText,
                        Caption = caption,
                        Italic = italic,
                        Bold = bold,
                        Underline = underline
                    });
                } 
                else
                {
                    if (cleanedNode.Name == "i" || cleanedNode.Name == "em")
                    {
                        italic = true;
                    }
                    if (cleanedNode.Name == "b" || cleanedNode.Name == "strong" || cleanedNode.InnerHtml.Contains("<b>"))
                    {
                        bold = true;
                    }
                    if (cleanedNode.Name == "u")
                    {
                        underline = true;
                    }

                    articleNodes.Add(new NodeModel
                    {
                        Type = type,
                        Content = cleanedNode.InnerText,
                        Italic = italic,
                        Bold = bold,
                        Underline = underline
                    });

                }

                italic = false;
                bold = false;
                italic = false;
                type = NodeType.Text;
            }

            foreach (NodeModel articleNode in articleNodes)
            {
                Console.WriteLine(JsonSerializer.Serialize(articleNode, new JsonSerializerOptions { WriteIndented = true }));
            }

            //foreach (var node in parsedNodes)
            //{
            //    var jsonString = JsonSerializer.Serialize(node, new JsonSerializerOptions { WriteIndented = true });
            //    Console.WriteLine(jsonString);
            //}
            //var textParts = Regex.Split(body.InnerHtml, "<br>");
            //List<string> cleanedTextParts = new List<string>();  
            //foreach (var part in textParts)
            //{
            //    if (!String.IsNullOrWhiteSpace(part) && part != "<br>" ) cleanedTextParts.Add(part.Trim());
            //}

            //List<string> pureNodes = new List<string>();    
            //foreach (var cleanedPart in cleanedTextParts)
            //{
            //    HtmlDocument tempDoc = new HtmlDocument();
            //    tempDoc.LoadHtml($"<div>{cleanedPart} </div>");
            //    HtmlNode modifiedNode = tempDoc.DocumentNode.FirstChild;
            //    if (modifiedNode.InnerText.Length != 0 && !modifiedNode.InnerText.Contains("<img")) 
            //    {
            //        pureNodes.Add(modifiedNode.InnerHtml);
            //    }
            //}

            //List<NodeModel> parsedNodes = new List<NodeModel>();
            //for (int i = 0; i < cleanedTextParts.Count; i++)
            //{
            //    ParseNode(cleanedTextParts[i], parsedNodes);
            //}

            //foreach (var node in parsedNodes)
            //{
            //    var jsonString = JsonSerializer.Serialize(node, new JsonSerializerOptions { WriteIndented = true });
            //    Console.WriteLine(jsonString);
            //}
        }

        internal static void ParseNode(string liNodeText, List<NodeModel> resultList)
        {
            var textParts = Regex.Split(liNodeText, "(<em>|</em>|<i>|</i>|<b>|</b>|<strong>|</strong>|<u>|</u>|<a href=\"[^\"]*\">|</a>|<img \"[^\"]*\")");
            NodeType nodeType = NodeType.Text;

            bool isItalic = false;
            bool isBold = false;    
            bool isUnderline = false;

            string currentLink = null;


            foreach (var textPart in textParts)
            {
                if (string.IsNullOrWhiteSpace(textPart)) continue;

                    switch (textPart)
                {
                    case "<em>":
                    case "<i>":
                        isItalic = true;
                        continue;
                    case "</em>":
                    case "</i>":
                        isItalic = false;
                        continue;
                    case "<strong>":
                    case "<b>":
                        isBold = true;
                        continue;
                    case "</strong>":
                    case "</b>":
                        isBold = false;
                        continue;
                    case "<u>":
                        isUnderline = true;
                        continue;
                    case "</u>":
                        isUnderline = false;
                        continue;
                    case var _ when textPart.StartsWith("<a href=\""):
                        nodeType = NodeType.Link; 
                        currentLink = Regex.Match(textPart, "<a href=\"([^\"]*)\">").Groups[1].Value;
                        continue;
                    case "</a>":
                        nodeType = NodeType.Text;
                        currentLink = null;
                        continue;
                }


                HtmlDocument tempDoc = new HtmlDocument();
                tempDoc.LoadHtml($"<div>{textPart} </div>");
                HtmlNode modifiedNode = tempDoc.DocumentNode.FirstChild;
                if (modifiedNode.Attributes["src"] != null)
                {
                    resultList.Add(new ImageNodeModel
                    {
                        Src = modifiedNode.Attributes["src"].Value,
                        AltText = modifiedNode?.Attributes["alt"].Value,
                        //Caption = modifiedNode.
                    });
                }

                if (string.IsNullOrWhiteSpace(modifiedNode.InnerText)) continue;

                resultList.Add(new NodeModel
                {
                    Content = modifiedNode.InnerText,
                    Italic = isItalic,
                    Bold = isBold,
                    Underline = isUnderline,
                    Link = currentLink,
                });
            }
        }

    }
}
