﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Helpers;
using TIAArticleAppAPI.Models;
using WebScraper.Validation;

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
                if (linkElement.IsNullOrBadLink()) continue;
                BaseArticleModel articleModel = GetBaseArticle(linkElement);
                if (articleLinks.Count < 10)
                {
                    articleLinks.Add(articleModel);
                }
            }

            foreach (HtmlNode linkElement in linkElements.Reverse())
            {
                if (linkElement.IsNullOrBadLink()) continue;
                BaseArticleModel articleModel = GetBaseArticle(linkElement);
                if (articleLinks.Count < 20)
                {
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
                HtmlNode descriptionNode = linkElement.SelectSingleNode(".//span");
                return new BaseArticleModel
                {
                    Url = HtmlParsingHelper.CleanLink(anchorNode.GetAttributeValue("href", ""), _url, true),
                    Title = anchorNode.InnerText.Trim(),
                    Description = descriptionNode?.InnerText.Trim(),
                };
            }
        }
    }
}
