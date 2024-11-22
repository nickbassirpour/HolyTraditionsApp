using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Helpers;
using WebScraper.Models;
using WebScraper.Validation;

namespace WebScraper.Services
{
    internal class ArticleListScraperService
    {
        private HtmlDocument _htmlDoc;
        private readonly string _category;
        internal ArticleListScraperService(string url)
        {
            HtmlWeb web = new HtmlWeb { OverrideEncoding = Encoding.UTF8 };
            _htmlDoc = web.Load(url);
            _category = _htmlDoc.DocumentNode.SelectSingleNode("//font[@size='6']").InnerText;
        }

        internal Result<List<BaseArticleModel>?, ValidationFailed> ScrapeArticles()
        {
            IEnumerable<HtmlNode> linkElements = GetLinkElements();
            if (linkElements.Count() == 0) return new ValidationFailed("Unable to scrape link elements");

            List<BaseArticleModel> articleLinks = new List<BaseArticleModel>();
            foreach (HtmlNode linkElement in linkElements)
            {
                if (linkElement.IsNullOrBadLink()) continue;
                BaseArticleModel articleModel = GetBaseArticle(linkElement);
                Console.WriteLine(articleModel.Title + "\n" + articleModel.Description);
                Console.WriteLine();

                articleLinks.Add(GetBaseArticle(linkElement));
            }

            return articleLinks;
        }

        private IEnumerable<HtmlNode> GetLinkElements()
        {
            IEnumerable<HtmlNode> linkElements = new List<HtmlNode>();

            if (_category.MatchesAnyOf(ScrapingHelper.linksWithNoDescription))
            {
                linkElements = _htmlDoc.DocumentNode.SelectNodes("//a");
            }
            else
            {
                linkElements = _htmlDoc.DocumentNode.SelectNodes("//td");
            }

            return linkElements;
        }

        private BaseArticleModel GetBaseArticle(HtmlNode linkElement)
        {
            if (_category.MatchesAnyOf(ScrapingHelper.linksWithNoDescription))
            {
                return new BaseArticleModel
                {
                    Url = linkElement.GetAttributeValue("href", ""),
                    Title = linkElement.InnerText,
                };
            }
            else
            {
                HtmlNode anchorNode = linkElement.SelectSingleNode(".//a");
                HtmlNode descriptionNode = linkElement.SelectSingleNode(".//span | .//font | .//br/following-sibling::text()");
                return new BaseArticleModel
                {
                    Url = linkElement.GetAttributeValue("href", ""),
                    Title = anchorNode.InnerText.Trim(),
                    Description = descriptionNode?.InnerText.Trim(),
                };
            }
        }
    }
}
