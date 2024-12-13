﻿using HtmlAgilityPack;
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
            if (article.Url.EndsWith(".pdf"))
            {
                continue;
            }
            // run and check for errors I'm the worst
            Scrape(article);
        }
    }

}
void Scrape(BaseArticleModel baseArticle)
{
    List<ArticleModel> articles = new List<ArticleModel>();
    ArticleScraperService webScraper = new ArticleScraperService(baseArticle.Url);
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

void ScrapeUrl(string url)
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


// Birds Eye View of the News
//ScrapeList("https://traditioninaction.org/bev/news.htm");

// Hot Topics
//ScrapeList("https://traditioninaction.org/HotTopics/R000-Transhumanism.html");
//ScrapeList("https://traditioninaction.org/HotTopics/a01Pedophilia_crisis.htm#crisis");
//ScrapeList("https://traditioninaction.org/HotTopics/a02Homo&Clergy.html#dancing");
//ScrapeList("https://traditioninaction.org/HotTopics/a00ConsequencesIndex.html#vatican");
//ScrapeList("https://traditioninaction.org/HotTopics/b00EcumenismIndex.html#ecumenism");
//ScrapeList("https://traditioninaction.org/HotTopics/b01CrisisInEcumenism.htm#crisis");
//ScrapeList("https://traditioninaction.org/HotTopics/i000htPoliticsIndex.html#affairs");
//ScrapeList("https://traditioninaction.org/HotTopics/j000htSocialPoliticalIndex.html#social");
//ScrapeList("https://traditioninaction.org/HotTopics/f000TradIssuesIndex.html#trad");
//ScrapeList("https://traditioninaction.org/HotTopics/g00FatimaIndex.html#fatima");
//ScrapeList("https://traditioninaction.org/HotTopics/d000htFeminismIndex.html#feminism");
//ScrapeList("https://traditioninaction.org/HotTopics/k00CanonizationsIndex.html#factory");
//ScrapeList("https://traditioninaction.org/HotTopics/e001htWarIndex.html#war");
ScrapeList("https://traditioninaction.org/HotTopics/c000ArtIndexht.html#art");
//Scrape("https://traditioninaction.org/religious/n002rpLapide_Appearance.htm");

