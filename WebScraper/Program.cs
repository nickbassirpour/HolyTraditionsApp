﻿using HtmlAgilityPack;
using System;
using System.Runtime.InteropServices;
using System.Text.Json;
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
        List<ArticleModel> scrapedArticles = new List<ArticleModel>();
        foreach (BaseArticleModel article in articles)
        {
            if (article.Url.EndsWith(".pdf") || article.Url.Contains("tiabk") || article.Url.EndsWith("pps") || article.Url.EndsWith("mp4"))
            {
                continue;
            }
            ArticleModel scrapedArticle = Scrape(article);
            if (scrapedArticle != null)
            {
                scrapedArticles.Add(scrapedArticle);
            }
        }
        Console.WriteLine("Article List Count: " + articles.Count());
        Console.WriteLine("Article Scrape Count: " + scrapedArticles.Count());
    }

}
ArticleModel? Scrape(BaseArticleModel baseArticle)
{
    ArticleScraperService webScraper = new ArticleScraperService(baseArticle);
    ArticleModel article = webScraper.ScrapeArticle();
    if (article != null)
    {
        Console.WriteLine();
        Console.WriteLine("Url: " + article.Url);
        Console.WriteLine("Author: " + article.Author?[0]);
        Console.WriteLine("Title: " + article.Title);
        //Console.WriteLine("ThumbnailURL: " + article.ThumbnailURL);
        //Console.WriteLine("Category: " + article.Category);
        //Console.WriteLine("SubCategory: " + article.SubCategory);
        //Console.WriteLine("Date: " + article.Date?.ToString());
        //Console.WriteLine("Series: " + article.Series);
        //Console.WriteLine("SeriesNumber: " + article.SeriesNumber);
        Console.WriteLine("Description: " + baseArticle.Description);
        //Console.WriteLine("RelatedArticles: " + article.RelatedArticles);

        return article;
    }
    return null;
}

var jsonLinks = System.IO.File.ReadAllText(@"C:\Users\nickb\Desktop\Code_Projects\TIABackend\WebScraper\Data\testLinks.json");
var links = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonLinks);

if (links != null)
{

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
            ScrapeList(url);
        }
    }
}

