using HtmlAgilityPack;
using System;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebScraper.Helpers;
using WebScraper.Models;
using WebScraper.Validation;

namespace WebScraper.Services
{
    internal class ArticleScraperService
    {
        private HtmlDocument _htmlDoc;
        private string _url;
        public ArticleScraperService(string url)
        {
            _url = url;

            HtmlWeb web = new HtmlWeb { OverrideEncoding = Encoding.UTF8 };
            _htmlDoc = web.Load(url);
        }

        public ArticleModel ScrapeArticle()
        {
            string splitHtmlBody = HtmlParsingHelper.SplitHtmlBody(_htmlDoc);

            ArticleModel articleModel = new ArticleModel();
            articleModel.Url = _url;
            articleModel.Category = HtmlParsingHelper.GetCategoryFromURL(_url);
            articleModel.SubCategory = GetSubCategory();
            articleModel.Series = GetSeries();
            articleModel.Title = GetTitle();
            articleModel.Author = GetAuthor();
            articleModel.BodyHtml = GetBody(splitHtmlBody);
            articleModel.BodyInnerText = GetBodyInnerText(splitHtmlBody);
            articleModel.Date = GetDate();
            articleModel.RelatedArticles = GetRelatedArticles(splitHtmlBody);
            return articleModel;
        }

        public string? GetSubCategory()
        {
            HtmlNode? topic = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null);
            if (topic == null)
            {
                return null;
            }
            return topic.InnerText.Trim();
        }

        private List<BaseArticleModel>? GetRelatedArticles(string htmlBody)
        {
            string bottomOfArticle = _htmlDoc.DocumentNode.InnerHtml.Split("Related Topics of Interest</div>")[1];
            List<BaseArticleModel> relatedArticles = new List<BaseArticleModel>();

            if (bottomOfArticle == null)
            {
                return null;
            }

            IEnumerable<HtmlNode> linkElements = HtmlParsingHelper.GetLinkElements(bottomOfArticle);

            if (linkElements.Count() == 0)
            {
                return null;
            }
   
            foreach (HtmlNode linkElement in linkElements)
            {
                if (String.IsNullOrWhiteSpace(linkElement.InnerText)) continue;
                if (linkElement.InnerText.MatchesAnyOf(ScrapingHelper.linksNotToScrape)) continue;

                relatedArticles.Add(new BaseArticleModel()
                {
                    Url = linkElement.GetAttributeValue("href", ""),
                    Title = linkElement.InnerText,
                });
            }
            
            return relatedArticles;
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

        public string GetBody(string htmlBody)
        {
            HtmlDocument htmlDocBody = HtmlParsingHelper.LoadHtmlDocument(htmlBody);
            HtmlNode parsedBody = HtmlParsingHelper.ParseBody(htmlDocBody.DocumentNode, _url);
            return parsedBody.InnerHtml;
        }

        private string GetBodyInnerText(string htmlBody)
        {
            HtmlDocument htmlDocBody = HtmlParsingHelper.LoadHtmlDocument(htmlBody);
            return htmlDocBody.DocumentNode.InnerText;
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
