﻿CREATE TABLE [dbo].[RelatedArticle]
(
	[Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	[ArticleId] INT NOT NULL,
	CONSTRAINT FK_Article_RelatedArticle FOREIGN KEY (ArticleId) REFERENCES Article(Id) ON DELETE CASCADE,
	[Url] VARCHAR(300) NOT NULL,
	[Title] NVARCHAR(300) NOT NULL,
	[ThumbnailUrl] VARCHAR(300),
	[Category] VARCHAR(100)
)


