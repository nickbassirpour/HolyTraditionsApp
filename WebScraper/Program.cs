using HtmlAgilityPack;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using WebScraper.Models;
using WebScraper.Enums;
using WebScraper.Services;

void ScrapeList(string url)
{
    ListScraperService articleListScraper = new ListScraperService(url);
    articleListScraper.ScrapeArticles();
}
async void Scrape(string url)
{
    ArticleScraperService webScraper = new ArticleScraperService(url);
    webScraper.ScrapeArticle(); 
}

ScrapeList("https://traditioninaction.org/religious/n000rpForgottenTruths.htm#forgotten");
//Scrape("https://traditioninaction.org/bev/298bev11_08_2024.htm");

