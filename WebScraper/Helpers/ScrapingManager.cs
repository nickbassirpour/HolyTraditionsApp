using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIAArticleAppAPI.Services;
using WebScraper.Services;

namespace WebScraper.Helpers
{
    internal class ScraperManager
    {
        private readonly IArticleService _service;
        internal ScraperManager(IArticleService service)
        {
            _service = service;
        }

        public async Task StartScraping(Dictionary<string, List<string>> links)
        {
            ListScraperService articleListScraper = new ListScraperService(_service);
            foreach (var category in links)
            {
                foreach (string url in category.Value)
                {
                    await Task.Delay(5000);
                    await articleListScraper.ScrapeList(url);
                }
            }
        }
    }
}
