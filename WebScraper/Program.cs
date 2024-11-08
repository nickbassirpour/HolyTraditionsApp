using HtmlAgilityPack;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using WebScraper.Models;
using WebScraper.Enums;
using WebScraper.Services;

void Scrape(string url)
{
    WebScraperService webScraper = new WebScraperService(url);

    string relatedArticles = webScraper.GetRelatedArticles();
    Console.WriteLine(relatedArticles);
}

Scrape("https://traditioninaction.org/HotTopics/i99ht_177_Dri.html");

