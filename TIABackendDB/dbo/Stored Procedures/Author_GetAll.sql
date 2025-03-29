CREATE PROCEDURE [dbo].[Author_GetAll]

AS
	BEGIN
		SELECT 
			[Name]
		FROM
			dbo.Author
	END
