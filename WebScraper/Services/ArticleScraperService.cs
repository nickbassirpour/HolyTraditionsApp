using HtmlAgilityPack;
using System;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebScraper.Helpers;
using WebScraper.Validation;
using TIAArticleAppAPI.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using Microsoft.AspNetCore.Builder;

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
            articleModel.Title = GetTitle(category);
            articleModel.Author = GetAuthor(category);
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

            HtmlNode? categoryFromTopicHeaderOrH3 = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null);
            if (categoryFromTopicHeaderOrH3 != null)
            {
                return categoryFromTopicHeaderOrH3.InnerText.Trim();
            }

            HtmlNode? categoryFromColorAndSize = _htmlDoc.DocumentNode.SelectSingleNode(".//*[@color='#800000' and @size='2']");
            if (categoryFromColorAndSize != null)
            {
                return categoryFromColorAndSize.InnerText.Trim();
            }

            return null;
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

        public List<string?> GetSeriesNameAndNumber()
        {
            HtmlNode? series = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='GreenSeries']");
            if (series == null || string.IsNullOrWhiteSpace(series.InnerText))
            {
                return null;
            }

            List<string> seriesParts = new List<string>();

            if (series.InnerText.Contains("-"))
            {
                seriesParts = series.InnerText.Trim().Split(" - ").ToList();
                return seriesParts;
            }

            if (series.InnerText.Contains("–"))
            {
                seriesParts = series.InnerText.Trim().Split(" – ").ToList();
                return seriesParts;
            }

            return null;
        }

        public string? GetTitle(string category)
        {
            if (category == "RevolutionPhotos")
            {
                HtmlNode? titleForRevolutionPhotos = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=4 and @color='#800000']");
                if (titleForRevolutionPhotos != null)
                {
                    return titleForRevolutionPhotos.InnerText.Trim();
                }

                HtmlNode? titleFromHTagsChurchRev = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Name == "h1");
                if (titleFromHTagsChurchRev != null)
                {
                    return titleFromHTagsChurchRev.InnerText.Trim();
                }
            }

            HtmlNode? titleFromHTags = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Name == "h1" || node.Name == "h4");
            if (titleFromHTags != null)
            {
                return titleFromHTags.InnerText.Trim();
            }

            HtmlNode? titleFromSizeAndColorMaroon = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=6 and @color='maroon' or @size=6 and @color='#800000']");
            if (titleFromSizeAndColorMaroon != null)
            {
                return titleFromSizeAndColorMaroon.InnerText.Trim();
            }

            HtmlNode? titleFromSizeAndColor99000 = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=6 and @color='#990000']");
            if (titleFromSizeAndColor99000 != null)
            {
                return titleFromSizeAndColor99000.InnerText.Trim();
            }

            HtmlNode? titleFromSizeAndColorGreen = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=6 and @color='green']");
            if (titleFromSizeAndColorGreen != null)
            {
                return titleFromSizeAndColorGreen.InnerText.Trim();
            }

            return null;
        }

        public List<string?> GetAuthor(string category)
        {
            if (category == "RevolutionPhotos")
            {
                return null;
            }

            if (category == "bev")
            {
                List<string> atilaAuthorList = new List<string> { "Atila S. Guimarães" };
                return atilaAuthorList;
            }

            HtmlNode? authorFromAuthorClass = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='author']");
            if (authorFromAuthorClass != null)
            {
                return SplitAuthors(authorFromAuthorClass);
            }

            HtmlNode? authorFromSizeAndId = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=4 or @size=4 and @id='R']");
            if (authorFromSizeAndId != null)
            {
                return SplitAuthors(authorFromSizeAndId);
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
                    string cleanedDateFromBEV = dateFromBEV.InnerText.Replace("NEWS:", "").Replace("News:", "").Replace("news:", "");
                    return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromBEV.Trim());
                }
                return null;
            }

            HtmlNode? dateFromId = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\'posted\' or @id=\'sitation\']");
            if (!string.IsNullOrWhiteSpace(dateFromId?.InnerText))
            {
                string cleanedDateFromId = dateFromId.InnerText;
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromId);
            }

            HtmlNode? dateFromSize1AndColorNavy = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size='1' and @color='navy'" +
            "and contains(text(), 'Posted')]");
            if (!string.IsNullOrWhiteSpace(dateFromSize1AndColorNavy?.InnerText))
            {
                string cleanedDateFromSizeAndColor = dateFromSize1AndColorNavy.InnerText;
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromSizeAndColor);
            }

            HtmlNode? dateFromSize1AndColorNAVY = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size='1' and @color='NAVY'" +
                "and contains(text(), 'Posted')]");
            if (!string.IsNullOrWhiteSpace(dateFromSize1AndColorNAVY?.InnerText))
            {
                string cleanedDateFromSizeAndColor = dateFromSize1AndColorNAVY.InnerText;
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromSizeAndColor);
            };

            HtmlNode? dateFromSize1AndColor000080 = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size='1' and @color='#000080'" +
                "and contains(text(), 'Posted')]");
            if (!string.IsNullOrWhiteSpace(dateFromSize1AndColor000080?.InnerText))
            {
                string cleanedDateFromSizeAndColor = dateFromSize1AndColor000080.InnerText;
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromSizeAndColor);
            };

            HtmlNode? dateFromPostedOnly = _htmlDoc.DocumentNode.SelectSingleNode("//*[contains(text(), 'Posted')]");
            if (!string.IsNullOrWhiteSpace(dateFromPostedOnly?.InnerText))
            {
                string cleanedDateFromSizeAndColor = dateFromSize1AndColorNAVY.InnerText;
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromSizeAndColor);
            };

            return null; 
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
