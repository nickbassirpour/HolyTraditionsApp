using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Models
{
    internal class BaseArticleModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public string? Description { get; set; }
    }
}
