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
        private BaseArticleModel _baseArticle;
        private string _url;
        public ArticleScraperService(BaseArticleModel baseArticle)
        {
            _baseArticle = baseArticle;
            _url = baseArticle.Url;
            HtmlWeb web = new HtmlWeb { OverrideEncoding = Encoding.UTF8 };
            _htmlDoc = web.Load(_url);
        }

        public ArticleModel ScrapeArticle()
        {
            string splitHtmlBody = HtmlParsingHelper.SplitHtmlBody(_htmlDoc, _url);

            if (splitHtmlBody == null)
            {
                return null;
            }
            string category = HtmlParsingHelper.GetCategoryFromURL(_url);

            // delete me
            Console.WriteLine();
            //Console.WriteLine(_url);

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
            HtmlNode? series = _htmlDoc.DocumentNode.SelectSingleNode("//*[@class='GreenSeries' or @class='greenseries']");
            if (series == null || string.IsNullOrWhiteSpace(series.InnerText))
            {
                return null;
            }

            List<string> seriesParts = new List<string>();

            if (series.InnerText.Contains("-"))
            {
                seriesParts = series.InnerText.Split("-").ToList();
                seriesParts[0].Replace("&nbsp;", "").Trim();
                seriesParts[1].Replace("&nbsp;", "").Trim();
                return seriesParts;
            }

            if (series.InnerText.Contains("–"))
            {
                seriesParts = series.InnerText.Split("–").ToList();
                seriesParts[0].Replace("&nbsp;", "").Trim();
                seriesParts[1].Replace("&nbsp;", "").Trim();
                return seriesParts;
            }

            return null;
        }

        public string? GetTitle(string category)
        {
            if (category == "bev")
            {
                return _baseArticle.Title;
            }

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

            HtmlNode? titleFromSizeAndColorMAROON = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=6 and @color='MAROON']");
            if (titleFromSizeAndColorMAROON != null)
            {
                return titleFromSizeAndColorMAROON.InnerText.Trim();
            }

            HtmlNode? titleFromSize5AndColor800000 = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=5 and @color='#800000']");
            if (titleFromSize5AndColor800000 != null)
            {
                return titleFromSize5AndColor800000.InnerText.Trim();
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
            if (authorFromAuthorClass != null && !String.IsNullOrWhiteSpace(authorFromAuthorClass.InnerText))
            {
                return SplitAuthors(authorFromAuthorClass);
            }

            HtmlNode? authorFromSizeAndId = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=4 and @id='R']");
            if (authorFromSizeAndId != null && !String.IsNullOrWhiteSpace(authorFromSizeAndId.InnerText))
            {
                return SplitAuthors(authorFromSizeAndId);
            }

            HtmlNode? authorFromSizeAndColor = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=4 and @color='PURPLE']");
            if (authorFromSizeAndColor != null && !String.IsNullOrWhiteSpace(authorFromSizeAndColor.InnerText))
            {
                return SplitAuthors(authorFromSizeAndColor);
            }

            HtmlNode? authorFromSizeOnly = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size=4]");
            if (authorFromSizeOnly != null && !String.IsNullOrWhiteSpace(authorFromSizeOnly.InnerText))
            {
                return SplitAuthors(authorFromSizeOnly);
            }

            if (_htmlDoc.DocumentNode.InnerText.ToLower().Contains("dr. horvat responds"))
            {
                return new List<string?> { "Dr. Marian Therese Horvat" };
            }

            if (_htmlDoc.DocumentNode.InnerText.ToLower().Contains("tia correspondence desk") ||
                _htmlDoc.DocumentNode.InnerText.ToLower().Contains("tia responds"))
            {
                return new List<string?> { "TIA Correspondence Desk" };
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
                return GetBevDate();
            }

            HtmlNode? dateFromId = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\'posted\' or @id=\'sitation\']");
            if (!string.IsNullOrWhiteSpace(dateFromId?.InnerText))
            {
                string cleanedDateFromId = dateFromId.InnerText;
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromId);
            }

            HtmlNode? dateElementFromSize1AndColorNavy = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size='1' and @color='navy']");
            if (!string.IsNullOrWhiteSpace(dateElementFromSize1AndColorNavy?.InnerText) && dateElementFromSize1AndColorNavy.InnerText.Contains("Posted"))
            {
                string cleanedDateFromSizeAndColor = dateElementFromSize1AndColorNavy.InnerText;
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromSizeAndColor);
            }

            HtmlNode? dateFromSize1AndColorNAVY = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size='1' and @color='NAVY']");
            if (!string.IsNullOrWhiteSpace(dateFromSize1AndColorNAVY?.InnerText) && dateFromSize1AndColorNAVY.InnerText.Contains("Posted"))
            {
                string cleanedDateFromSizeAndColor = dateFromSize1AndColorNAVY.InnerText;
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromSizeAndColor);
            };

            HtmlNode? dateFromSize1AndColor000080 = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size='1' and @color='#000080']");
            if (!string.IsNullOrWhiteSpace(dateFromSize1AndColor000080?.InnerText) && dateFromSize1AndColor000080.InnerText.Contains("Posted"))
            {
                string cleanedDateFromSizeAndColor = dateFromSize1AndColor000080.InnerText;
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromSizeAndColor);
            };

            HtmlNode? dateFromPostedOnly = _htmlDoc.DocumentNode.SelectSingleNode("//*[contains(text(), 'Posted')]");
            if (!string.IsNullOrWhiteSpace(dateFromPostedOnly?.InnerText))
            {
                string cleanedDateFromPostedText = dateFromPostedOnly.InnerText;
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromPostedText);
            };

            return null; 
        }

        private string? GetBevDate()
        {
            HtmlNode? dateFromBEV = null;
            if (_htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null) != null)
            {
                dateFromBEV = _htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null);
            }
            else if (_htmlDoc.DocumentNode.SelectSingleNode("//*[@size='2' and @color='#800000' or @size='2' and @color='maroon']") != null)
            {
                // add logic to find first or default date at top (descendants, attrib same, text contains posted). 
                dateFromBEV = _htmlDoc.DocumentNode.SelectSingleNode("//*[@size='2' and @color='#800000' or @size='2' and @color='maroon']");
            }
            if (!string.IsNullOrWhiteSpace(dateFromBEV?.InnerText))
            {
                string cleanedDateFromBEV = dateFromBEV.InnerText.Replace("NEWS:", "").Replace("News:", "").Replace("news:", "");
                return HtmlParsingHelper.ConvertStringToDate(cleanedDateFromBEV.Trim());
            }
            return null;
        }

        public string? GetThumbnailUrl(string splitHtmlBody)
        {
            HtmlDocument splitBodyNode = HtmlParsingHelper.LoadHtmlDocument(splitHtmlBody);
            HtmlNode? firstImageNode = splitBodyNode.DocumentNode.SelectSingleNode("(//img)[1]");
            if (firstImageNode == null) return null;
            
            if (firstImageNode.GetAttributeValue("src", string.Empty).MatchesAnyOf(ScrapingHelper.skipFirstThumbnailImage))
            {
                firstImageNode = splitBodyNode.DocumentNode.SelectSingleNode("(//img)[2]");
            }

            string src = firstImageNode.GetAttributeValue("src", string.Empty);
            string srcWithDomain = HtmlParsingHelper.CleanLink(src, _url, true);
            return srcWithDomain;
        }
    }
}
