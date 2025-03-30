CREATE PROCEDURE [dbo].[Category_GetAll]

AS
	BEGIN
		SELECT 
			[Name]
		FROM
			Category
	END
