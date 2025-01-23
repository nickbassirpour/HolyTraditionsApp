using TIAArticleAppAPI.Models;

namespace TIAArticleAppAPI.Services
{
    public interface IArticleService
    {
        void AddNewArticle(ArticleModel article);
        ArticleModel GetArticleByUrl(string url);
        List<BaseArticleModel> GetArticleListByCategory(string category);
    }
}