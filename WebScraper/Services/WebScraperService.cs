using HtmlAgilityPack;
using System;
using System.Text;
using System.Text.RegularExpressions;
using WebScraper.Helpers;
using WebScraper.Models;
using WebScraper.Validation;

namespace WebScraper.Services
{
    internal class WebScraperService
    {
        private HtmlDocument _htmlDoc;
        private string _url;
        public WebScraperService(string url)
        {
            _url = url;

            HtmlWeb web = new HtmlWeb { OverrideEncoding = Encoding.UTF8 };
            _htmlDoc = web.Load(url);
        }

        public async Task<Result<ArticleModel, ValidationFailed>> ScrapeArticle()
        {
            string htmlBody = SplitHtmlBody();

            ArticleModel articleModel = new ArticleModel();
            articleModel.Url = _url;
            articleModel.Topic = GetTopic();
            articleModel.Series = GetSeries();
            articleModel.Title = GetTitle();
            articleModel.Author = GetAuthor();
            articleModel.Body = GetBody(htmlBody);
            articleModel.Date = GetDate();
            return articleModel;
        }

        public string? GetTopic()
        {
            HtmlNode? topic = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null);
            if (topic == null)
            {
                return null;
            }
            return topic.InnerText.Trim();
        }

        public string? GetSeries()
        {
            HtmlNode? series = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='GreenSeries']");
            if (series == null)
            {
                return null;
            }
            return series.InnerText.Trim();
        }

        public string? GetTitle()
        {
            HtmlNode? title = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Name == "h1" || node.Name == "h4");
            if (title == null)
            {
                return null;
            }
            return title.InnerText.Trim();
        }

        public string? GetAuthor()
        {
            HtmlNode? author = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='author']");
            if (author == null)
            {
                return null;
            }
            return author.InnerText.Trim();
        }

        public string SplitHtmlBody()
        {
            string htmlBodyNode = _htmlDoc.DocumentNode.InnerHtml;
            string splitHtmlBody = htmlBodyNode.Split("alt=\"contact\">")[1];
            string cleanedHtmlBody = String.Join("", Regex.Split(splitHtmlBody, @"<!-- AddToAny BEGIN -->.*?<!-- AddToAny END -->", RegexOptions.Singleline));

            Console.WriteLine(cleanedHtmlBody);

            return cleanedHtmlBody;
        }

        public string GetBody(string htmlBody)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlBody);

            HtmlParsingHelper.ParseBody(document.DocumentNode, _url);
            string hello = "hello";
            return hello;
        }

        public string? GetDate()
        {
            HtmlNode? date = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id='posted' or @id='sitation' or contains(.//text(), 'posted on')]");
            if (date == null)
            {
                return null;
            }
            return date.InnerText.Trim();
        }

    }
}
