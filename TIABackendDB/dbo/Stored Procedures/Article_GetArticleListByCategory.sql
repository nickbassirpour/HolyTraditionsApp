CREATE PROCEDURE [dbo].[Article_GetArticleListByCategory]

	@Category VARCHAR(100),
	@Limit int

AS
BEGIN
		SELECT TOP 
			(@Limit) * 
		FROM 
			Article a
		INNER JOIN
			SubCategory sc
		ON
			a.SubCategoryId = sc.Id
		INNER JOIN
			Category c
		ON
			sc.CategoryId = c.Id
		WHERE 
			c.[Name] = @Category
		ORDER BY 
			a.[Date]
END;
