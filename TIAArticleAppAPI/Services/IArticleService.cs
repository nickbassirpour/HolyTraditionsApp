﻿using TIAArticleAppAPI.Models;
using TIAArticleAppAPI.Validation;

namespace TIAArticleAppAPI.Services
{
    public interface IArticleService
    {
        Task<Result<int?, ValidationFailed>> AddNewArticle(ArticleModel article);
        Task<Result<ArticleModel, ValidationFailed>> GetArticleByUrl(string url);
        Task<Result<List<BaseArticleModel>, ValidationFailed>> GetArticleListByCategory(string category, int limit);
        Task<Result<List<BaseArticleModel>, ValidationFailed>> GetArticleListBySubcategory(string subcategory, int limit);
        Task<Result<ArticleModel, ValidationFailed>> GetArticleById(int articleId);
        Task<Result<List<string>, ValidationFailed>> GetAllAuthors();
        Task<Result<List<string>, ValidationFailed>> GetAllCategories();
        Task<Result<List<string>, ValidationFailed>> GetAllSubCategories();
        Task<Result<(int, string), ValidationFailed>> DeleteArticleById(int articleId);
        Task<IEnumerable<ArticleModel>> SearchArticles(int? categoryId = null, int? subCategoryId = null, int? authorId = null, string searchTerm = null);
    }
}