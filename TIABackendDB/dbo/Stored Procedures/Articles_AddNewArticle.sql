CREATE PROCEDURE [dbo].[Articles_AddNewArticle]
	@title varchar(200),
	@url varchar(300),
	@category varchar(100),
	@description varchar(300),
	@subcategory varchar(100),
	@series varchar(200),
	@seriesNumber varchar(20),
	@author varchar(200),
	@bodyHtml varchar(MAX),
	@bodyInnerText varchar(MAX),
	@date date,
	@relatedArticles varchar(MAX)
	
AS
	SELECT @param1, @param2
RETURN 0
