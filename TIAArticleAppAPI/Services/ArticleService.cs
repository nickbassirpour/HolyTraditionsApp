﻿using TIAArticleAppAPI.Models;
using TIAArticleAppAPI.Data;
using Dapper;
using System.Data;
using TIAArticleAppAPI.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.Json;

namespace TIAArticleAppAPI.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ISqlDataAccess _db;

        public ArticleService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Result<List<BaseArticleModel>, ValidationFailed>> GetArticleListByCategory(string category, int limit)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Category", category);
            parameters.Add("@Limit", limit);

            return await _db.LoadDataList<BaseArticleModel, DynamicParameters>("dbo.Article_GetArticleListByCategory", parameters);
        }

        public async Task<Result<List<BaseArticleModel>, ValidationFailed>> GetArticleListBySubcategory(string subcategory, int limit)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Subcategory", subcategory);
            parameters.Add("@Limit", limit);

            return await _db.LoadDataList<BaseArticleModel, DynamicParameters>("dbo.Article_GetArticleListBySubcategory", parameters);
        }

        public async Task<Result<ArticleModel, ValidationFailed>> GetArticleById(int articleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ArticleId", articleId);

            var article = await _db.LoadDataObject<ArticleModel, DynamicParameters>("dbo.Article_GetArticleById", parameters);

            article.Author = DeserializeList<string>(article.AuthorJson);
            article.RelatedArticles = DeserializeList<BaseArticleModel>(article.RelatedArticlesJson);

            return article;
        }

        public async Task<Result<ArticleModel, ValidationFailed>> GetArticleByUrl(string articleUrl)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Url", articleUrl);

            var article = await _db.LoadDataObject<ArticleModel, DynamicParameters>("dbo.Article_GetArticleByUrl", parameters);

            if (article.AuthorJson != null) article.Author = DeserializeList<string?>(article.AuthorJson);
            if (article.RelatedArticlesJson != null) article.RelatedArticles = DeserializeList<BaseArticleModel>(article.RelatedArticlesJson);

            return article;
        }

        public async Task<Result<List<string>, ValidationFailed>> GetAllAuthors()
        {
            return await _db.LoadDataList<string>("dbo.Author_GetAll");
        }

        public async Task<Result<List<string>, ValidationFailed>> GetAllCategories()
        {
            return await _db.LoadDataList<string>("Category_GetAll");
        }
        public async Task<Result<List<string>, ValidationFailed>> GetAllSubCategories()
        {
            return await _db.LoadDataList<string>("SubCategory_GetAll");
        }
        public async Task<Result<(int, string), ValidationFailed>> DeleteArticleById(int articleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ArticleId", articleId);

            var article = await _db.LoadDataObject<(int Id, string Title), DynamicParameters>("dbo.Article_DeleteArticleById", parameters);

            return article;
        }

        public async Task<IEnumerable<ArticleModel>> SearchArticles(int? categoryId = null, int? subCategoryId = null, int? authorId = null, string searchTerm = null)
        {
            string sql = @"
                    SELECT DISTINCT 
                    a.*,
                    sc.Name as SubCategory,
                    c.Name as Category,
                    '[' + STRING_AGG(QUOTENAME(au.Name, '""'), ', ') + ']' as authorJson
                    FROM Article a 
                    INNER JOIN SubCategory sc on a.SubCategoryId = sc.Id
                    INNER JOIN Category c on sc.CategoryId = c.Id
                    INNER JOIN Author_Article aa on a.Id = aa.ArticleId
                    INNER JOIN Author au ON aa.AuthorId = au.id
                    WHERE 1 = 1";

            DynamicParameters parameters = new DynamicParameters();

            if (categoryId != null)
            {
                sql += " AND c.Id = @CategoryId";
                parameters.Add("CategoryId", categoryId);
            }

            if (subCategoryId != null)
            {
                sql += " AND sc.Id = @SubCategoryId";
                parameters.Add("SubCategoryId", categoryId);
            }

            if (authorId != null)
            {
                sql += " AND au.Id = @AuthorId";
                parameters.Add("AuthorId", authorId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                sql += " AND (a.Title LIKE @SearchTerm OR a.BodyInnerText LIKE @SearchTerm)";
                parameters.Add("SearchTerm", $"%{searchTerm}%");
            }

            sql += " GROUP BY a.Id, a.Title, a.Url, a.BodyHtml, a.BodyInnerText, a.SubCategoryId, a.Description, a.ThumbnailURL, c.[Name], sc.[Name], a.[Date]";

            IEnumerable<ArticleModel> articles = await _db.QueryRawSql<ArticleModel, DynamicParameters>(sql, parameters);

            foreach (ArticleModel article in articles)
            {
                if (article.AuthorJson != null)
                {
                    article.Author = DeserializeList<string?>(article.AuthorJson);
                }
            }

            return articles;
        }

        public async Task<Result<int?, ValidationFailed>> AddNewArticle(ArticleModel article)
        {
            // Log each property before adding to parameters
            Debug.WriteLine($"SubCategory: {article.SubCategory}");
            Debug.WriteLine($"Title: {article.Title}");
            Debug.WriteLine($"Url: {article.Url}");
            Debug.WriteLine($"Category: {article.Category}");
            Debug.WriteLine($"Description: {article.Description}");
            Debug.WriteLine($"ThumbnailUrl: {article.ThumbnailURL}");
            Debug.WriteLine($"Series: {article.Series}");
            Debug.WriteLine($"SeriesNumber: {article.SeriesNumber}");
            Debug.WriteLine($"BodyHtml: {article.BodyHtml != null}");
            Debug.WriteLine($"BodyInnerText: {article.BodyInnerText != null}");
            Debug.WriteLine($"Date: {article.Date}");

            var parameters = new DynamicParameters();
            parameters.Add("@SubCategory", article.SubCategory);
            parameters.Add("@Title", article.Title);
            parameters.Add("@Url", article.Url);
            parameters.Add("@Category", article.Category);
            parameters.Add("@Description", article.Description);
            parameters.Add("@ThumbnailUrl", article.ThumbnailURL);
            parameters.Add("@Series", article.Series);
            parameters.Add("@SeriesNumber", article.SeriesNumber);

            if (article.Author?.Any() == true)
            {
                DataTable authorTable = ConvertAuthorsToDataTable(article.Author);
                parameters.Add("@Authors", authorTable.AsTableValuedParameter("AuthorListType"));
                Debug.WriteLine("Authors:");
                foreach (var author in article.Author)
                {
                    Debug.WriteLine($" - {author}");
                }
            }
            else
            {
                parameters.Add("@Authors", new DataTable().AsTableValuedParameter("AuthorListType"));
                Debug.WriteLine("Authors: NULL or Empty");
            }

            parameters.Add("@BodyHtml", article.BodyHtml);
            parameters.Add("@BodyInnerText", article.BodyInnerText);
            parameters.Add("@Date", article.Date);


            if (article.RelatedArticles?.Any() == true)
            {
                DataTable relatedArticleTable = ConvertRelatedArticlesToDataTable(article.RelatedArticles);
                parameters.Add("@RelatedArticles", relatedArticleTable.AsTableValuedParameter("RelatedArticleListType"));
                Debug.WriteLine("Related Articles:");
                foreach (var related in article.RelatedArticles)
                {
                    Debug.WriteLine($" - {related.Url}, {related.Title}");
                }
            }
            else
            {
                parameters.Add("@RelatedArticles", new DataTable().AsTableValuedParameter("RelatedArticleListType"));
                Debug.WriteLine("Related Articles: NULL or Empty");
            }

            parameters.Add("@NewArticleId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            try
            {
                await _db.SaveData<DynamicParameters>("dbo.Article_AddNewArticle", parameters);
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Exception: {ex.Message}");
                Debug.WriteLine($"Error Code: {ex.Number}");
                foreach (SqlError error in ex.Errors)
                {
                    Debug.WriteLine($"Error: {error.Number} - {error.Message}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Exception: {ex.Message}");
            }

            int? newArticleId = parameters.Get<int?>("@NewArticleId");

            if (!newArticleId.HasValue) return new ValidationFailed("Failed to save article");

            return newArticleId.Value;
        }

        private static DataTable? ConvertAuthorsToDataTable(List<String> authors)
        {
            if (authors == null) return null;
            DataTable authorTable = new DataTable();
            authorTable.Columns.Add("Name", typeof(string));

            foreach (var author in authors)
            {
                authorTable.Rows.Add(author);
            }
            return authorTable;
        }
        private static DataTable? ConvertRelatedArticlesToDataTable(List<BaseArticleModel> relatedArticles)
        {
            if (relatedArticles == null) return null;
            DataTable relatedArticlesTable = new DataTable();
            relatedArticlesTable.Columns.Add("Title", typeof(string));
            relatedArticlesTable.Columns.Add("Url", typeof(string));

            foreach (var relatedArticle in relatedArticles)
            {
                relatedArticlesTable.Rows.Add(relatedArticle.Title, relatedArticle.Url);
            }
            return relatedArticlesTable;
        }

        private static List<T> DeserializeList<T>(string json)
        {
            return string.IsNullOrEmpty(json)
            ? new List<T>()
            : JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
    }
}
