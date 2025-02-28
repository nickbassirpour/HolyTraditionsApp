CREATE TYPE RelatedArticleListType AS TABLE
(
	Title NVARCHAR(300),
	Url VARCHAR(MAX),
	ThumbnailUrl NVARCHAR(MAX),
	Category VARCHAR(100)
)
