CREATE TABLE [dbo].[Articles]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Title] VARCHAR(200) NOT NULL,
	[Url] VARCHAR(300) NOT NULL,
	[Description] VARCHAR(300),
	[SubCategoryId] INT NOT NULL,
	CONSTRAINT FK_Article_SubCategory FOREIGN KEY (SubCategoryId) REFERENCES SubCategory(Id),
	[Series] VARCHAR(300),
	[Author] VARCHAR(200),
	[BodyHtml] VARCHAR(MAX) NOT NULL,
	[BodyInnerText] VARCHAR(MAX) NOT NULL,
	[Date] DATE,
	[RelatedArticles] VARCHAR(MAX)
)
