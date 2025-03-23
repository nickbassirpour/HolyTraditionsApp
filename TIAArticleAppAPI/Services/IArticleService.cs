using TIAArticleAppAPI.Models;
using TIAArticleAppAPI.Validation;

namespace TIAArticleAppAPI.Services
{
    public interface IArticleService
    {
        Task<Result<int?, ValidationFailed>> AddNewArticle(ArticleModel article);
        Task<ArticleModel> GetArticleByUrl(string url);
        Task<Result<List<BaseArticleModel>, ValidationFailed>> GetArticleListByCategory(string category, int limit);
        Task<Result<List<BaseArticleModel>, ValidationFailed>> GetArticleListBySubcategory(string subcategory, int limit);
        Task<Result<ArticleModel, ValidationFailed>> GetArticleById(int articleId);
    }
}