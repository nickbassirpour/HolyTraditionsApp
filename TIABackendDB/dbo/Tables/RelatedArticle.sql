CREATE TABLE [dbo].[RelatedArticle]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[ArticleId] INT NOT NULL,
	CONSTRAINT FK_Article_RelatedArticle FOREIGN KEY (ArticleId) REFERENCES RelatedArticle(Id),
	[Url] VARCHAR(500) NOT NULL,
	[Title] NVARCHAR(MAX) NOT NULL,
	[ThumbnailUrl] VARCHAR(500),
	[Category] VARCHAR(100)
)


