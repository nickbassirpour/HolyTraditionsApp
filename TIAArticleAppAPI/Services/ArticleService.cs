using TIAArticleAppAPI.Models;
using TIAArticleAppAPI.Data;
using Dapper;
using System.Data;
using TIAArticleAppAPI.Validation;
using System.Data.SqlClient;
using System.Diagnostics;

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
                Debug.WriteLine("Before calling SaveData");
                await _db.SaveData<DynamicParameters>("dbo.Articles_AddNewArticle", parameters);
                Debug.WriteLine("After calling SaveData");
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
    }
}
