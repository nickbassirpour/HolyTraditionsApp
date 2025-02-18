CREATE PROCEDURE [dbo].[Articles_AddNewArticle]
	@subcategory varchar(100), --add subcategory to category table
	@title varchar(200),
	@url varchar(300),
	@category varchar(100), --add category to category table
	@description varchar(300),
	@thumbnailUrl varchar(MAX),
	@series varchar(200), --add series to series table
	@seriesNumber varchar(20),
	@author varchar(200), --add author to author table
	@bodyHtml nvarchar(MAX),
	@bodyInnerText nvarchar(MAX),
	@date date,
	@relatedArticles nvarchar(MAX)
	
AS
BEGIN
	
	SET NOCOUNT ON;

	BEGIN 

	INSERT INTO 
		dbo.Articles 
			(

			)
RETURN 0
