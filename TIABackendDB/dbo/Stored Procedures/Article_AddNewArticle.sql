CREATE PROCEDURE [dbo].[Article_AddNewArticle]
	@SubCategory varchar(100),
	@Title varchar(200),
	@Url varchar(300),
	@Category varchar(100),
	@Description varchar(300),
	@ThumbnailUrl varchar(MAX),
	@Series varchar(200),
	@SeriesNumber varchar(20),
	@Authors AuthorListType READONLY,
	@BodyHtml nvarchar(MAX),
	@BodyInnerText nvarchar(MAX),
	@Date date,
	@RelatedArticles RelatedArticleListType READONLY,
	@NewArticleId INT OUTPUT
	
AS
BEGIN
	IF EXISTS (SELECT 1 FROM dbo.Article WHERE Url = @Url)
	BEGIN
		RAISERROR('Article with this URL already exists.', 16, 1);
		RETURN;
	END
	
	SET NOCOUNT ON;

	DECLARE @CategoryId INT;
	
	SELECT 
		@CategoryId = c.Id
	FROM 
		dbo.Category c 
	WHERE
		Name = @Category

	IF @CategoryId IS NULL
	
		BEGIN
			INSERT INTO 
				dbo.Category 
					([Name])
				VALUES
					(@Category)

			SET @CategoryId = SCOPE_IDENTITY()
		END

		
	DECLARE @SubCategoryId INT;

	SELECT
		@SubCategoryId = sc.Id
	FROM 
		dbo.SubCategory sc
	WHERE 
		[Name] = @SubCategory
	AND
		CategoryId = @CategoryId

	IF @SubCategoryId IS NULL

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

	IF EXISTS (SELECT 1 FROM @Authors)
	BEGIN
	
		DECLARE @AuthorIds TABLE (Id INT);

		INSERT INTO 
				dbo.Author ([Name])
			OUTPUT 
				INSERTED.Id INTO @AuthorIds
			SELECT 
				[Name]
			FROM 
				@Authors
			WHERE 
				NOT EXISTS 
					(
					SELECT 
						1 
					FROM 
						dbo.Author a
					WHERE 
						[Name] = a.Name
					);

		INSERT INTO 
				@AuthorIds (Id)
			SELECT 
				Id 
			FROM 
				dbo.Author 
			WHERE
				[Name]
			IN 
				(
				SELECT 
					[Name] 
				FROM 
					@Authors
				)
			AND 
				Id
			NOT IN 
				(
				SELECT 
					Id 
				FROM
					@AuthorIds
				);

		END
			

	DECLARE @ArticleId INT;

	INSERT INTO 
		dbo.Article 
		(
		[Title], 
		[Url], 
		[Description], 
		[ThumbnailUrl], 
		[BodyHtml], 
		[BodyInnerText], 
		[Date], 
		[SubCategoryId]
		)
		VALUES 
		(
		@Title,
		@Url, 
		@Description, 
		@ThumbnailUrl, 
		@BodyHtml, 
		@BodyInnerText, 
		@Date, 
		@SubCategoryId
		);
	SET @ArticleId = SCOPE_IDENTITY();

	INSERT INTO 
		dbo.Author_Article 
		(
		ArticleId, 
		AuthorId
		)
	SELECT 
		@ArticleId, 
		ai.Id 
	FROM 
		@AuthorIds ai;

	IF @Series IS NOT NULL
	BEGIN

		DECLARE @SeriesId INT;

		SELECT 
			@SeriesId = s.Id 
		FROM 
			dbo.Series s
		WHERE 
			[Name] = @Series;
	
		IF @SeriesId IS NULL 

		BEGIN
			INSERT INTO 
				dbo.Series 
					([Name])
				VALUES
					(@Series)
			SET 
				@SeriesId = SCOPE_IDENTITY();
		END

		INSERT INTO 
			dbo.Series_Articles
			(
			SeriesId, 
			SeriesNumber, 
			ArticleId
			)
			VALUES
			(
			@SeriesId, 
			@SeriesNumber, 
			@ArticleId
			)

	END

	IF EXISTS (SELECT 1 FROM @RelatedArticles)
	BEGIN

		INSERT INTO 
			dbo.RelatedArticle 
			(
			[Title], 
			[Url], 
			[ArticleId]
			)
		SELECT
			ra.[Title], 
			ra.[Url], 
			@ArticleId 
		FROM 
			@RelatedArticles ra
	END

	SET @NewArticleId = @ArticleId;

	RETURN 0;

END;
