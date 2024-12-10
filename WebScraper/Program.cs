using HtmlAgilityPack;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using TIAArticleAppAPI.Models;
using WebScraper.Enums;
using WebScraper.Services;
using WebScraper.Validation;

void ScrapeList(string url)
{
    ListScraperService articleListScraper = new ListScraperService(url);
    List<BaseArticleModel> articles = articleListScraper.ScrapeArticles();
    if (articles != null)
    {
        foreach (BaseArticleModel article in articles)
        {
            // run and check for errors
            Scrape(article.Url);
        }
    }

}
void Scrape(string url)
{
    List<ArticleModel> articles = new List<ArticleModel>();
    ArticleScraperService webScraper = new ArticleScraperService(url);
    ArticleModel article = webScraper.ScrapeArticle();
    if (article != null)
    {
        Console.WriteLine(article.Url);
        Console.WriteLine(article.Author);
        Console.WriteLine(article.Title);
        Console.WriteLine(article.ThumbnailURL);
        Console.WriteLine(article.Category);
        Console.WriteLine(article.SubCategory);
        Console.WriteLine(article.Date);
        Console.WriteLine(article.Series);
        Console.WriteLine(article.SeriesNumber);
        Console.WriteLine(article.Description);
        Console.WriteLine(article.RelatedArticles);
    
        articles.Add(article); 
    }
}

ScrapeList("https://traditioninaction.org/religious/n000rpForgottenTruths.htm#forgotten");
//Scrape("https://traditioninaction.org/bev/298bev11_08_2024.htm");

