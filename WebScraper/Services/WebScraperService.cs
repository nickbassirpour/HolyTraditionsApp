using HtmlAgilityPack;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Helpers;
using WebScraper.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebScraper.Services
{
    internal class WebScraperService
    {
        private HtmlDocument _htmlDoc;
        public WebScraperService(string url)
        {
            HtmlWeb web = new HtmlWeb { OverrideEncoding = Encoding.UTF8 };
            _htmlDoc = web.Load(url);
        }

        public string GetTopic()
        {
            var topic = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null);
            return topic.InnerText.Trim();
        }

        public string GetSeries()
        {
            var series = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='GreenSeries']");
            return series.InnerText.Trim();
        }

        public string GetTitle()
        {
            var title = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Name == "h1" || node.Name == "h4");
            return title.InnerText.Trim();
        }

        public string GetAuthor()
        {
            var author = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='author']");
            return author.InnerText.Trim();
        }
        public void GetBody()
        {
            HtmlNode body = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id='R']");
            HtmlDocument splitBody = new HtmlDocument();
            splitBody.LoadHtml(body.InnerHtml.Split("<!-- Add")[0]);
            HtmlNode splitBodyHtmlNode = splitBody.DocumentNode;

            //HtmlNode? commentNode = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(n => n.InnerHtml.Contains("<-- Add"));

            //if (commentNode != null)
            //{
            //    commentNode.Remove();

            //    HtmlNode sibling = commentNode.NextSibling;
            //    while (sibling != null)
            //    {
            //        HtmlNode nextSibling = sibling.NextSibling;
            //        sibling.Remove();
            //        sibling = nextSibling;
            //    }
            //}
            List<HtmlNode> cleanedNodes = new List<HtmlNode>();
            var nodes = splitBodyHtmlNode.ChildNodes;

            for (int i = 0; i < nodes.Count; i++) 
            {
                if (!string.IsNullOrEmpty(nodes[i].InnerText.Trim())) cleanedNodes.Add(nodes[i]);
            }

            for (int i = 0;i < cleanedNodes.Count; i++)
            {
                Console.WriteLine(i.ToString() + ") " + cleanedNodes[i].InnerHtml);
            }
            List<List<NodeModel>> parsedBody = new List<List<NodeModel>>();

            //foreach (var body in bodyList)
            //{
            //    var cleanedBody = body.InnerHtml.Split("<!-- Add")[0];
            //    HtmlDocument tempDoc = new HtmlDocument();
            //    tempDoc.LoadHtml($"<div>{ cleanedBody} </div>");
            //    HtmlNode modifiedNode = tempDoc.DocumentNode.FirstChild;
            //    var nodes = modifiedNode.Descendants();
            //    foreach (var node in nodes)
            //    {
            //        Console.WriteLine(node.InnerHtml);
            //    }
            //    //HtmlParsingHelper.ParseBody(modifiedNode);

            //}
            //return HtmlParsingHelper.ParseBody(body);
        }

        public void GetSource()
        {

        }

        public void GetContinued()
        {

        }

        public List<NodeModel> GetFootnotes()
        {
            var footNotes = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id='footnotes']");
            return HtmlParsingHelper.ParseListItems(footNotes);
        }

        public string GetDate()
        {
            var date = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id='posted' or @id='sitation']");
            return date.InnerText.Trim();
        }

        //public List<NodeModel> GetRelatedArticles()
        //{
        //    var relatedArticles = _htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='relatedlist']");
        //    return HtmlParsingHelper.ParseLinks(relatedArticles);
        //}
    }
}
