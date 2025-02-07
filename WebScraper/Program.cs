using System;
using System.Text.Json;
using TIAArticleAppAPI.Data;
using TIAArticleAppAPI.Models;
using TIAArticleAppAPI.Services;
using WebScraper.Helpers;
using WebScraper.Services;



//async void BeginScrape(string url)
//{

//    ListScraperService articleListScraper = new ListScraperService(url);
//}
//async void ScrapeList(string url)
//{
//    ListScraperService articleListScraper = new ListScraperService(url);
//    List<BaseArticleModel> articlesFromList = articleListScraper.ScrapeArticles();
//    if (articlesFromList != null)
//    {
//        List<ArticleModel> scrapedArticles = new List<ArticleModel>();
//        List<BaseArticleModel> notScrapedArticles = new List<BaseArticleModel>();
//        foreach (BaseArticleModel article in articlesFromList)
//        {
//            if (article.Url.EndsWith(".pdf") || article.Url.Contains("tiabk") || article.Url.EndsWith("pps") || article.Url.EndsWith("mp4"))
//            {
//                continue;
//            }
//            ArticleModel scrapedArticle = Scrape(article);
//            if (scrapedArticle != null)
//            {
//                scrapedArticles.Add(scrapedArticle);
//            }
//            else
//            {
//                notScrapedArticles.Add(article);
//            }
//        }
//        Console.WriteLine();
//        Console.WriteLine("Article List Count: " + articlesFromList.Count());
//        Console.WriteLine("Article Scrape Count: " + scrapedArticles.Count());
//        Console.WriteLine();
//        foreach (BaseArticleModel notScrapedArticle in notScrapedArticles)
//        {
//            Console.WriteLine(notScrapedArticle.Url);
//            Console.WriteLine(notScrapedArticle.Title);
//            Console.WriteLine(notScrapedArticle.Description);
//            Console.WriteLine();
//        }
//    }

//}
//ArticleModel? Scrape(BaseArticleModel baseArticle)
//{
//    ArticleScraperService webScraper = new ArticleScraperService(baseArticle);
//    ArticleModel article = webScraper.ScrapeArticle();
//    if (article != null)
//    {
//        Console.WriteLine();
//        Console.WriteLine("Url: " + article.Url);
//        Console.WriteLine("Author: " + article.Author?[0]);
//        Console.WriteLine("Title: " + article.Title);
//        //Console.WriteLine("ThumbnailURL: " + article.ThumbnailURL);
//        //Console.WriteLine("Category: " + article.Category);
//        //Console.WriteLine("SubCategory: " + article.SubCategory);
//        //Console.WriteLine("Date: " + article.Date?.ToString());
//        //Console.WriteLine("Series: " + article.Series);
//        //Console.WriteLine("SeriesNumber: " + article.SeriesNumber);
//        Console.WriteLine("Description: " + baseArticle.Description);
//        //Console.WriteLine("RelatedArticles: " + article.RelatedArticles);

//        return article;
//    }
//    return null;
//}

var jsonLinks = System.IO.File.ReadAllText(@"C:\Users\nickb\Desktop\Code_Projects\TIABackend\WebScraper\Data\testLinks.json");
var links = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonLinks);

if (links != null)
{
    SqlDataAccess db = new SqlDataAccess();
    IArticleService articleService = new ArticleService(db); // Replace with actual implementation
    ScraperManager scraperManager = new ScraperManager(articleService);

    foreach (var category in links)
    {
        string categoryName = category.Key;
        //if (categoryName != "religiousTopics")
        //{
        //    continue;
        //}
        List<string> urls = category.Value;

        //ScrapeList(urls[0]);

        foreach (string url in urls)
        {
            await Task.Delay(5000);
            BeginScrape(url);
        }
    }
}

