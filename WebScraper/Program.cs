using HtmlAgilityPack;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using DataAccessLibrary.Models;
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
            string fullUrl = "https://traditioninaction.org/" + article.Url;
            Scrape(fullUrl);
        }
    }

}
async void Scrape(string url)
{
    ArticleScraperService webScraper = new ArticleScraperService(url);
    webScraper.ScrapeArticle(); 
}

ScrapeList("https://traditioninaction.org/religious/n000rpForgottenTruths.htm#forgotten");
//Scrape("https://traditioninaction.org/bev/298bev11_08_2024.htm");

