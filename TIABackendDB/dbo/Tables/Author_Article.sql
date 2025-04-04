CREATE TABLE [dbo].[Author_Article]
(
	[AuthorId] INT NOT NULL,
	[ArticleId] INT NOT NULL,
	CONSTRAINT PK_Author_Article PRIMARY KEY (AuthorId, ArticleId),
	CONSTRAINT FK_Author FOREIGN KEY (AuthorId) REFERENCES Author(Id),
	CONSTRAINT FK_Article FOREIGN KEY (ArticleId) REFERENCES Article(Id) ON DELETE CASCADE
)
