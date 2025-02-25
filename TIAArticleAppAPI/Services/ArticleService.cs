using TIAArticleAppAPI.Models;
using TIAArticleAppAPI.Data;
using Dapper;
using System.Data;
using TIAArticleAppAPI.Validation;

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
            var parameters = new DynamicParameters();
            parameters.Add("@SubCategory", article.SubCategory);
            parameters.Add("@Title", article.Title);
            parameters.Add("@Url", article.Url);
            parameters.Add("@Category", article.Category);
            parameters.Add("@Description", article.Description);
            parameters.Add("@ThumbnailURL", article.ThumbnailURL);
            parameters.Add("@Series", article.Series);
            parameters.Add("@SeriesNumber", article.SeriesNumber);

            DataTable authorTable = ConvertAuthorsToDataTable(article.Author);
            if (authorTable != null) parameters.Add("@Authors", authorTable.AsTableValuedParameter("AuthorListType"));
            else parameters.Add("@Authors", null);

            parameters.Add("@BodyHtml", article.BodyHtml);
            parameters.Add("@BodyInnerText", article.BodyInnerText);
            parameters.Add("@Date", article.Date);
            parameters.Add("@RelatedArticles", article.RelatedArticles);
            parameters.Add("@NewArticleId", dbType: DbType.Int32, direction: ParameterDirection.Output); 

            try
            {
                await _db.SaveData<DynamicParameters>("dbo.Articles_AddNewArticle", parameters);
            }
            catch (Exception ex)
            {
                string message = ex.Message;   
                Console.WriteLine(message);
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
    }
}
