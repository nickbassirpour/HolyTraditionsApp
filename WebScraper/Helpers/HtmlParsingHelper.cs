using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
                string liNodeText = liNode.InnerHtml;
                liNodeText.Replace("</li>", "").Replace("<li>", "");

                FindItalics(liNodeText, resultList);
                FindBold(liNodeText, resultList);
                FindUnderline(liNodeText, resultList);
            }
            return resultList;
        }

        internal static void FindItalics(string liNodeText, List<NodeModel> resultList)
        {
            var textParts = Regex.Split(liNodeText, "(<em>|</em>)");
            bool isItalic = false;

            foreach (var textPart in textParts)
            {
                if (textPart == "<em>")
                {
                    isItalic = true;
                    continue;
                }
                else if (textPart == "</em>")
                {
                    isItalic = false;
                    continue;
                }

                resultList.Add(new NodeModel
                {
                    Content = textPart,
                    Italic = isItalic,
                });
            }
        }

        internal static void FindBold(string liNodeText, List<NodeModel> resultList)
        {
            var textParts = Regex.Split(liNodeText, "(<b>|</b>)");
            bool isBold = false;    

            foreach (var textPart in textParts)
            {
                if (textPart == "<b>")
                {
                    isBold = true;
                    continue;
                }
                else if (textPart == "</b>")
                {
                    isBold= false;
                    continue;
                }

                resultList.Add(new NodeModel
                {
                    Content = textPart,
                    Bold = isBold,
                });
            }
        }

        internal static void FindUnderline(string liNodeText, List<NodeModel> resultList)
        {
            var textParts = Regex.Split(liNodeText, "(<u>|</u>)");
            bool isBold = false;

            foreach (var textPart in textParts)
            {
                if (textPart == "<u>")
                {
                    isBold = true;
                    continue;
                }
                else if (textPart == "</u>")
                {
                    isBold = false;
                    continue;
                }

                resultList.Add(new NodeModel
                {
                    Content = textPart,
                    Bold = isBold,
                });
            }
        }
    }
}
