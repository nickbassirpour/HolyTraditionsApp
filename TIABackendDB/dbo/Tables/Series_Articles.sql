CREATE TABLE [dbo].[Series_Articles]
(
	[SeriesId] INT NOT NULL,
	[ArticleId] INT NOT NULL,
	[SeriesNumber] VARCHAR(10) NOT NULL,
	PRIMARY Key (SeriesId, ArticleId),
	FOREIGN Key (SeriesId) REFERENCES Series(Id) ON DELETE CASCADE,
	FOREIGN Key (ArticleId) REFERENCES Article(Id) ON DELETE CASCADE
)
