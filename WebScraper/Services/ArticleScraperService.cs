﻿using HtmlAgilityPack;
using System;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebScraper.Helpers;
using WebScraper.Validation;
using TIAArticleAppAPI.Models;

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

            if (splitHtmlBody == null)
            {
                return null;
            }
            string category = HtmlParsingHelper.GetCategoryFromURL(_url);

            ArticleModel articleModel = new ArticleModel();
            articleModel.Url = _url;
            articleModel.Category = category;
            articleModel.SubCategory = GetSubCategory(category);
            articleModel.Series = GetSeriesNameAndNumber() != null ? GetSeriesNameAndNumber()[0] : null;
            articleModel.SeriesNumber = GetSeriesNameAndNumber() != null ? GetSeriesNameAndNumber()[1] : null;
            articleModel.Title = GetTitle();
            articleModel.Author = GetAuthor();
            articleModel.BodyHtml = GetBody(splitHtmlBody);
            articleModel.BodyInnerText = GetBodyInnerText(splitHtmlBody);
            articleModel.ThumbnailURL = GetThumbnailUrl(splitHtmlBody);
            articleModel.Date = GetDate(category);
            articleModel.RelatedArticles = GetRelatedArticles(splitHtmlBody);
            return articleModel;
        }

        public string? GetSubCategory(string category)
        {
            if (category.MatchesAnyOf(ScrapingHelper.categoriesWithNoSubcatregory))
            {
                return category;
            }

            HtmlNode? topic = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null);
            if (topic == null)
            {
                return null;
            }
            return topic.InnerText.Trim();
        }

        private List<BaseArticleModel>? GetRelatedArticles(string htmlBody)
        {
            List<string> splitArticleParts = _htmlDoc.DocumentNode.InnerHtml.Split("Related Topics of Interest").ToList();

            if (splitArticleParts.Count() == 1)
            {
                return null;
            }

            List<BaseArticleModel> relatedArticles = new List<BaseArticleModel>();
            IEnumerable<HtmlNode> linkElements = HtmlParsingHelper.GetLinkElements(splitArticleParts[1]);

            if (linkElements.Count() == 0)
            {
                return null;
            }
   
            foreach (HtmlNode linkElement in linkElements)
            {
                if (String.IsNullOrWhiteSpace(linkElement.InnerText)) continue;
                if (linkElement.InnerText.MatchesAnyOf(ScrapingHelper.linkTextsNotToScrape)) continue;

                relatedArticles.Add(new BaseArticleModel()
                {
                    Url = HtmlParsingHelper.CleanLink(linkElement.GetAttributeValue("href", ""), _url, true),
                    Title = linkElement.InnerText,
                });
            }
            
            return relatedArticles;
        }

        public string?[] GetSeriesNameAndNumber()
        {
            HtmlNode? series = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='GreenSeries']");
            if (series == null || string.IsNullOrWhiteSpace(series.InnerText))
            {
                return null;
            }
            string[] seriesParts = series.InnerText.Trim().Split(" - ");
            return seriesParts;
        }

        public string? GetTitle()
        {
            HtmlNode? titleFromHTags = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Name == "h1" || node.Name == "h4");
            if (titleFromHTags != null)
            {
                return titleFromHTags.InnerText.Trim();
            }

            HtmlNode? titleFromSizeAndColor = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=6 and @color='maroon' or @size=6 and @color='#800000']");
            if (titleFromSizeAndColor != null)
            {
                return titleFromSizeAndColor.InnerText.Trim();
            }

            return null;
        }

        public List<string?> GetAuthor()
        {
            HtmlNode? authorFromAuthorClass = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='author']");
            if (authorFromAuthorClass != null)
            {
                return SplitAuthors(authorFromAuthorClass);
            }

            HtmlNode? authorFromSizeAndColor = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=4 and @color='PURPLE']");
            if (authorFromSizeAndColor != null)
            {
                return SplitAuthors(authorFromSizeAndColor);
            }

            return null;
        }

        public List<string> SplitAuthors(HtmlNode authorText)
        {
            return authorText.InnerText.Trim()
                .Split(new[] { "and", ",", "&" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(author => author.Trim())
                .ToList();
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

        public string? GetDate(string category)
        {
            if (category == "bev")
            {
                HtmlNode? dateFromBEV = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null);
                if (!string.IsNullOrWhiteSpace(dateFromBEV?.InnerText))
                {
                    string cleanedDateFromBEV = dateFromBEV.InnerText.Replace("NEWS:", "").Replace("News:", "").Replace("news:", "").Trim();
                    return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromBEV);
                }
                return null;
            }

            HtmlNode? dateFromId = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\'posted\' or @id=\'sitation\']");
            if (!string.IsNullOrWhiteSpace(dateFromId?.InnerText))
            {
                string cleanedDateFromId = dateFromId.InnerText.Trim();
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromId);
            }

            HtmlNode? dateFromSizeAndColor = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size='1' and @color='navy' " +
                "and contains(text(), 'Posted') or contains(text(), 'posted')]");
            if (!string.IsNullOrWhiteSpace(dateFromSizeAndColor?.InnerText))
            {
                string cleanedDateFromSizeAndColor = dateFromSizeAndColor.InnerText.Trim();
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromSizeAndColor);
            }

            return null; //or nothing
        }

        public string? GetThumbnailUrl(string splitHtmlBody)
        {
            HtmlDocument splitBodyNode = HtmlParsingHelper.LoadHtmlDocument(splitHtmlBody);
            HtmlNode? firstImageUrl = splitBodyNode.DocumentNode.SelectSingleNode("//img[1]");
            if (firstImageUrl == null)
            {
                return null;
            }
            string src = firstImageUrl.GetAttributeValue("src", string.Empty);
            string srcWithDomain = HtmlParsingHelper.AddDomainToLink(src, _url, true);
            return srcWithDomain;
        }
    }
}
