using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class ArticleModel : BaseArticleModel
    {
        public string SubCategory { get; set; }
        public string? Series {  get; set; }
        public string? Author { get; set; }
        public string BodyHtml { get; set; }
        public string BodyInnerText { get; set; }
        public string? Date { get; set; }
        public List<BaseArticleModel>? RelatedArticles { get; set; }

    }
}
