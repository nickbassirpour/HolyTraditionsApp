using HtmlAgilityPack;
using System.Text;
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
            HtmlWeb web = new HtmlWeb { OverrideEncoding = Encoding.UTF8 };
            _url = url;
            _htmlDoc = web.Load(url);
        }

        public ArticleModel ScrapeArticle()
        {
            (string htmlBody, string htmlEndOfArticle) = SplitHtmlBody();

            ArticleModel articleModel = new ArticleModel();
            articleModel.Url = new System.Uri(_url);
            articleModel.Topic = GetTopic();
            articleModel.Series = GetSeries();
            articleModel.Title = GetTitle();
            articleModel.Author = GetAuthor();
            articleModel.Body = GetBody(htmlBody);
            articleModel.Date = GetDate();
            return articleModel;
        }

        public string GetTopic()
        {
            HtmlNode? topic = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null);
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

        public string GetTitle()
        {
            HtmlNode? title = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Name == "h1" || node.Name == "h4");
            return title.InnerText.Trim();
        }

        public string GetAuthor()
        {
            HtmlNode? author = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='author']");
            return author.InnerText.Trim();
        }

        public (string, string) SplitHtmlBody()
        {
            string htmlBodyNode = _htmlDoc.DocumentNode.InnerHtml;
            string splitHtmlBody = htmlBodyNode.Split("alt=\"contact\">")[1];
            List<string> cleanedHtmlBody = splitHtmlBody.Split("<!-- AddToAny BEGIN -->").ToList();

            string htmlBody = cleanedHtmlBody[0];
            string htmlEndOfArticle = cleanedHtmlBody[1];

            return (htmlBody, htmlEndOfArticle);
        }
        public string GetBody(string htmlBody)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlBody);

            HtmlParsingHelper.ParseBody(document.DocumentNode);
            string hello = "hello";
            return hello;
        }

        public void GetSource()
        {

        }

        public string GetDate()
        {
            HtmlNode? date = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id='posted' or @id='sitation']");
            return date.InnerText.Trim();
        }

        //public List<NodeModel> GetRelatedArticles()
        //{
        //    var relatedArticles = _htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='relatedlist']");
        //    return HtmlParsingHelper.ParseLinks(relatedArticles);
        //}
    }
}
