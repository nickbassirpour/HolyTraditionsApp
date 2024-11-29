CREATE TABLE [dbo].[Article]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Title] VARCHAR(200) NOT NULL,
	[Url] VARCHAR(300) NOT NULL,
	[Description] VARCHAR(300),
	[SubCategoryId] INT NOT NULL,
	CONSTRAINT FK_Article_SubCategory FOREIGN KEY (SubCategoryId) REFERENCES SubCategory(Id),
	[SeriesId] INT NOT NULL,
	CONSTRAINT FK_Article_Series FOREIGN KEY (SeriesId) REFERENCES Series(Id),
	[BodyHtml] VARCHAR(MAX) NOT NULL,
	[BodyInnerText] VARCHAR(MAX) NOT NULL,
	[Date] DATE,
	[RelatedArticles] VARCHAR(MAX)
)
