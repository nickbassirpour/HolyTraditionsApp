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
            // run and check for errors I'm the worst
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
        Console.WriteLine();
        Console.WriteLine("Url: " + article.Url);
        Console.WriteLine("Author: " + article.Author?[0]);
        Console.WriteLine("Title: " + article.Title);
        Console.WriteLine("ThumbnailURL: " + article.ThumbnailURL);
        Console.WriteLine("Category: " + article.Category);
        Console.WriteLine("SubCategory: " + article.SubCategory);
        Console.WriteLine("Date: " + article.Date?.ToString());
        Console.WriteLine("Series: " + article.Series);
        Console.WriteLine("SeriesNumber: " + article.SeriesNumber);
        Console.WriteLine("Description: " + article.Description);
        Console.WriteLine("RelatedArticles: " + article.RelatedArticles);
    
        articles.Add(article); //add proper validfation
    }
}

ScrapeList("https://traditioninaction.org/religious/n000rpForgottenTruths.htm#forgotten");
//Scrape("https://traditioninaction.org/bev/298bev11_08_2024.htm");

