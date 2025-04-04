CREATE PROCEDURE [dbo].[Article_DeleteArticleById]
	@ArticleId int
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @DeletedArticle TABLE (Id INT, Title NVARCHAR(300));

    DELETE FROM Article
    OUTPUT deleted.Id, deleted.Title INTO @DeletedArticle
    WHERE Id = @ArticleId;

    SELECT * FROM @DeletedArticle;
END