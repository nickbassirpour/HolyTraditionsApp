CREATE PROCEDURE [dbo].[Articles_AddNewArticle]
	@SubCategory varchar(100), --add subcategory to category table
	@Title varchar(200),
	@Url varchar(300),
	@Category varchar(100), --add category to category table
	@Description varchar(300),
	@ThumbnailUrl varchar(MAX),
	@Series varchar(200), --add series to series table
	@SeriesNumber varchar(20),
	@Authors AuthorListType READONLY, --add author to author table
	@BodyHtml nvarchar(MAX),
	@BodyInnerText nvarchar(MAX),
	@Date date,
	@RelatedArticles nvarchar(MAX)
	
AS
BEGIN
	
	SET NOCOUNT ON;

	DECLARE @CategoryId INT;
	DECLARE @SubCategoryId INT;

		IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE [Name] = @Category)
		BEGIN
			INSERT INTO 
				dbo.Category 
					([Name])
				VALUES
					(@category)

			SET @CategoryId = SCOPE_IDENTITY()
		END
		ELSE
		BEGIN
			SELECT @CategoryId = Id FROM dbo.Category WHERE [Name] = @Category;
		END

		IF NOT EXISTS (SELECT 1 FROM dbo.SubCategory WHERE [Name] = @SubCategory AND CategoryId = @CategoryId)
		BEGIN
			INSERT INTO
				dbo.SubCategory
					(
					[Name],
					[CategoryId]
					)
				VALUES 
					(
					@subcategory,
					@CategoryId
					)
				SET @SubCategoryId = SCOPE_IDENTITY()
		END
		ELSE
		BEGIN
			SELECT @SubCategoryId = Id FROM dbo.SubCategory WHERE [Name] = @SubCategory AND CategoryId = @CategoryId;
		END

		DECLARE @AuthorIds TABLE (Id INT);

		INSERT INTO
			dbo.Author
				(
				[NAME]
				)
			OUTPUT INSERTED.Id INTO @AuthorIds
			SELECT 
				a.Name
			FROM
				@Authors a
				WHERE NOT EXISTS (SELECT 1 from dbo.Authors WHERE Name = a.Name)



RETURN 0

END;
