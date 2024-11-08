﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Helpers;
using WebScraper.Models;

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

        public List<NodeModel> GetRelatedArticles()
        {
            var relatedArticles = _htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='relatedlist']");
            return HtmlParsingHelper.ParseLinks(relatedArticles);
        }
    }
}
