using HtmlAgilityPack;
using System.Text;
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

        public ArticleModel ScrapeArticle()
        {
            ArticleModel articleModel = new ArticleModel();
            articleModel.Author = GetAuthor();
            articleModel.Title = GetTitle();
            articleModel.Topic = GetTopic();
            //articleModel.Source = GetSource();
            articleModel.Series = GetSeries();
            articleModel.Date = GetDate();
            return articleModel;
        }

        public string GetTopic()
        {
            HtmlNode? topic = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null);
            return topic.InnerText.Trim();
        }

        public string GetSeries()
        {
            HtmlNode? series = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='GreenSeries']");
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
        public void GetBody()
        {
            HtmlNode body = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id='R']");
            HtmlDocument splitBody = new HtmlDocument();
            splitBody.LoadHtml(body.InnerHtml.Split("<!-- Add")[0]);
            HtmlNode splitBodyHtmlNode = splitBody.DocumentNode;

            HtmlParsingHelper.ParseBody(splitBodyHtmlNode);
        }

        public void GetSource()
        {

        }

        public void GetContinued()
        {

        }

        public List<NodeModel> GetFootnotes()
        {
            HtmlNode? footNotes = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id='footnotes']");
            return HtmlParsingHelper.ParseListItems(footNotes);
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
