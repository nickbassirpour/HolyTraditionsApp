using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Models
{
    internal class ArticleModel : BaseArticleModel
    {
        public string Topic { get; set; }
        public string? Series {  get; set; }
        public string? Author { get; set; }
        public string Body { get; set; }
        public string? Source { get; set; }
        public string? Date { get; set; }
        public string? RelatedArticles { get; set; }
    }
}
