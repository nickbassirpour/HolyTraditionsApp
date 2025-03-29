CREATE PROCEDURE [dbo].[Article_GetArticleListBySubcategory]
	@Subcategory VARCHAR(100),
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
	WHERE
		sc.[Name] = @Subcategory
	ORDER BY
		a.[Date]
END
