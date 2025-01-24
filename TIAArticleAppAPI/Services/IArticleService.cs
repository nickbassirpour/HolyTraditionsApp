using TIAArticleAppAPI.Models;

namespace TIAArticleAppAPI.Services
{
    public interface IArticleService
    {
        Task<int?> AddNewArticle(ArticleModel article);
        ArticleModel GetArticleByUrl(string url);
        List<BaseArticleModel> GetArticleListByCategory(string category);
    }
}