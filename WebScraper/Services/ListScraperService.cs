using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Helpers;
using TIAArticleAppAPI.Models;
using WebScraper.Validation;
using System.Data.Common;

namespace WebScraper.Services
{
    internal class ListScraperService
    {
        private readonly string _url;
        private HtmlDocument _htmlDoc;
        private readonly string _category;
        internal ListScraperService(string url)
        {
            HtmlWeb web = new HtmlWeb { OverrideEncoding = Encoding.UTF8 };
            _url = url;
            _htmlDoc = web.Load(url);
            _category = _htmlDoc.DocumentNode.SelectSingleNode("//font[@size='6' or @size='7']").InnerText;
        }

        internal List<BaseArticleModel>? ScrapeArticles()
        {
            IEnumerable<HtmlNode> linkElements = GetLinkElements();
            if (linkElements.Count() == 0) return null;

            List<BaseArticleModel> articleLinks = new List<BaseArticleModel>();
            foreach (HtmlNode linkElement in linkElements)
            {
                if (linkElement.IsSeries())
                {
                    List<BaseArticleModel> articleModels = GetBaseArticleListFromSeries(linkElement);
                    articleLinks.AddRange(articleModels);
                } else
                {
                    if (linkElement.IsNullOrBadLink()) continue;
                    BaseArticleModel articleModel = GetBaseArticle(linkElement);
                    articleLinks.Add(articleModel);
                }
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
                HtmlNode descriptionNode = linkElement.SelectSingleNode(".//span") 
                    ?? linkElement.SelectSingleNode(".//*[@size='3' and @color='MAROON']")
                    ?? linkElement.SelectSingleNode(".//*[@size='3']")
                    ?? linkElement.SelectSingleNode(".//*[@color='#FF0000']").SelectSingleNode("text()[normalize-space()]");

                return new BaseArticleModel
                {
                    Url = HtmlParsingHelper.CleanLink(anchorNode.GetAttributeValue("href", ""), _url, true),
                    Title = anchorNode.InnerText.Trim(),
                    Description = descriptionNode?.InnerText.Trim(),
                };
            }
        }


        private List<BaseArticleModel> GetBaseArticleListFromSeries(HtmlNode linkElement)
        {
            var bElementDoc = new HtmlDocument();
            bElementDoc.LoadHtml(linkElement.InnerHtml);
            IEnumerable<HtmlNode> bElements = bElementDoc.DocumentNode.SelectNodes("//b");
            List<BaseArticleModel> baseArticleModelList = new List<BaseArticleModel>();
            if (bElements != null)
            {
                BaseArticleModel baseArticleModel = new BaseArticleModel();
                foreach (HtmlNode bElement in bElements)
                {
                    HtmlNode anchorNode = linkElement.SelectSingleNode(".//a");
                    if (anchorNode != null)
                    {
                        baseArticleModel.Url = HtmlParsingHelper.CleanLink(anchorNode.GetAttributeValue("href", ""), _url, true);
                        baseArticleModel.Title = anchorNode.InnerText.Trim();
                        baseArticleModel.Description = bElement.SelectSingleNode("following-sibling::*[@color='#800000']")?.InnerText.Trim();
                        //baseArticleModel.Description = bElement.SelectSingleNode("following-sibling::text()")?.InnerText.Trim();
                    }
                }
                baseArticleModelList.Add(baseArticleModel);
            }
            return baseArticleModelList;
        }
    }
}
