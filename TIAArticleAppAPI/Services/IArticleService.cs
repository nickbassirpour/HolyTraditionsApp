using TIAArticleAppAPI.Models;
using TIAArticleAppAPI.Validation;

namespace TIAArticleAppAPI.Services
{
    public interface IArticleService
    {
        Task<Result<int?, ValidationFailed>> AddNewArticle(ArticleModel article);
        ArticleModel GetArticleByUrl(string url);
        Task<Result<List<BaseArticleModel>, ValidationFailed>> GetArticleListByCategory(string category, int limit);
    }
}