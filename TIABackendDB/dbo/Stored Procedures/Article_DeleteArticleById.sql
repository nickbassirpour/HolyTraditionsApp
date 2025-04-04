CREATE PROCEDURE [dbo].[Article_DeleteArticleById]
	@ArticleId int
AS
BEGIN
	DELETE FROM Article WHERE Id = @ArticleId;
END