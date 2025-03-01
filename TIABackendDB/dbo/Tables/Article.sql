CREATE TABLE [dbo].[Article]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Title] NVARCHAR(300) NOT NULL,
	[Url] VARCHAR(500) NOT NULL,
	[Description] NVARCHAR(300),
	[SubCategoryId] INT NOT NULL,
	CONSTRAINT FK_Article_SubCategory FOREIGN KEY (SubCategoryId) REFERENCES SubCategory(Id),
	[ThumbnailUrl] VARCHAR(500),
	CONSTRAINT FK_Article_Series FOREIGN KEY (SeriesId) REFERENCES Series(Id),
	[BodyHtml] VARCHAR(MAX) NOT NULL,
	[BodyInnerText] VARCHAR(MAX) NOT NULL,
	[Date] DATE
)
