CREATE PROCEDURE [dbo].[Articles_AddNewArticle]
	@title varchar(200),
	@url varchar(300),
	@category varchar(100), --add category to category table
	@description varchar(300),
	@subcategory varchar(100), --add subcategory to category table
	@series varchar(200), --add series to series table
	@seriesNumber varchar(20),
	@author varchar(200), --add author to author table
	@bodyHtml varchar(MAX),
	@bodyInnerText varchar(MAX),
	@date date,
	@relatedArticles varchar(MAX)
	
AS
BEGIN
	
	SET NOCOUNT ON;

	BEGIN 

	INSERT INTO 
		dbo.Articles 
			(

			)
RETURN 0
