CREATE PROCEDURE [dbo].[Article_GetArticleByUrl]
	@Url VARCHAR(500)
AS
BEGIN
	SELECT 
		a.*,
		'[' + STRING_AGG(QUOTENAME(au.Name, '"'), ', ') + ']' as authorJson,
		c.[Name] as "category",
		sc.[Name] as "subCategory",
		COALESCE((
			SELECT
				Url,
				Title,
				ThumbnailUrl,
				Category,
				Description
			FROM
				RelatedArticle
			WHERE
				RelatedArticle.ArticleId = a.Id
			FOR 
				JSON PATH
		), '[]') as relatedArticlesJson
	FROM 
		Article a
	INNER JOIN
		Author_Article aa
	ON
		a.Id = aa.ArticleId
	INNER JOIN
		Author au
	ON
		au.Id = aa.AuthorId
	INNER JOIN
		SubCategory sc
	ON 
		sc.Id = a.SubCategoryId
	INNER JOIN
		Category c
	ON
		c.Id = sc.CategoryId
	WHERE
		a.Url = @Url
	GROUP BY
		 a.Id, a.Title, a.Url, a.BodyHtml, a.BodyInnerText, a.SubCategoryId,
        a.Description, a.ThumbnailURL, c.[Name], sc.[Name], a.[Date]
END

