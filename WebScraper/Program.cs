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

    webScraper.GetFootnotes();
    webScraper.GetTitle();
    string topic = webScraper.GetTopic();
    Console.WriteLine(topic);
}

Scrape("https://traditioninaction.org/bev/298bev11_08_2024.htm");

