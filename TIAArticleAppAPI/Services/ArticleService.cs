using TIAArticleAppAPI.Models;
using TIAArticleAppAPI.Data;

namespace TIAArticleAppAPI.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ISqlDataAccess _db;

        public ArticleService(ISqlDataAccess db)
        {
            _db = db;
        }
        public ArticleModel GetArticleByUrl(string url)
        {
            return _db.LoadDataObject<ArticleModel, dynamic>("dbo.Articles_GetArticleByUrl", new { url });
        }
        public List<BaseArticleModel> GetArticleListByCategory(string category)
        {
            return _db.LoadDataList<BaseArticleModel, dynamic>("dbo.Articles_GetArticleListByCategory", new { category });
        }
        public void AddNewArticle(ArticleModel article)
        {
            _db.SaveData<ArticleModel>("dbo.Articles_AddNewArticle", article);
        }
    }
}
