using TIAArticleAppAPI.Models;
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
            var parameters = new DynamicParameters();

            parameters.Add("@SubCategory", article.SubCategory);
            parameters.Add("@Title", article.Title);
            parameters.Add("@Url", article.Url);
            parameters.Add("@Category", article.Category);
            parameters.Add("@Description", article.Description);
            parameters.Add("@ThumbnailUrl", article.ThumbnailURL);
            parameters.Add("@Series", article.Series);
            parameters.Add("@SeriesNumber", article.SeriesNumber);
            parameters.Add("@BodyHtml", article.BodyHtml);
            parameters.Add("@BodyInnerText", article.BodyInnerText);
            parameters.Add("@Date", article.Date);

            DataTable emptyAuthorTable = CreateDataTable(["Name"]);
            if (article.Author?.Any() == true)
            {
                DataTable filledAuthorTable = AddAuthorsToTable(emptyAuthorTable, article.Author);
                parameters.Add("@Authors", filledAuthorTable.AsTableValuedParameter("AuthorListType"));
            }
            else
            {
                parameters.Add("@Authors", emptyAuthorTable.AsTableValuedParameter("AuthorListType"));
            }


            DataTable emptyRelatedArticlesTable = CreateDataTable(["Url", "Title"]);
            if (article.RelatedArticles?.Any() == true)
            {
                DataTable relatedArticleTable = AddRelatedArticlesToTable(emptyRelatedArticlesTable, article.RelatedArticles);
                parameters.Add("@RelatedArticles", relatedArticleTable.AsTableValuedParameter("RelatedArticleListType"));
            }
            else
            {
                parameters.Add("@RelatedArticles", emptyRelatedArticlesTable.AsTableValuedParameter("RelatedArticleListType"));
            }

            parameters.Add("@NewArticleId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            try
            {
                await _db.SaveData<DynamicParameters>("dbo.Article_AddNewArticle", parameters);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    return new ValidationFailed("Article already exists.");
                }

                Debug.WriteLine($"SQL Exception: {ex.Message}");
                return new ValidationFailed("Unhandled SQL error.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Exception: {ex.Message}");
                return new ValidationFailed("Unhandled exception.");
            }

            int? newArticleId = parameters.Get<int?>("@NewArticleId");

            if (!newArticleId.HasValue) return new ValidationFailed("Failed to save article");

            return newArticleId.Value;
        }

        private static DataTable? AddAuthorsToTable(DataTable authorTable, List<string> authors)
        {
            foreach (var author in authors)
            {
                authorTable.Rows.Add(author);
            }
            return authorTable;
        }
        private static DataTable? AddRelatedArticlesToTable(DataTable relatedArticlesTable, List<BaseArticleModel> relatedArticles)
        {
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

        private static DataTable CreateDataTable(List<string> columns)
        {
            DataTable table = new DataTable();
            foreach (string column in columns)
            {
                table.Columns.Add(column, typeof(string));
            }
            return table;
        }
    }
}
