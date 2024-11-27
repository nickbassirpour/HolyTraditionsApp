CREATE TABLE [dbo].[SubCategory]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] VARCHAR(100) NOT NULL,
	[CategoryId] INT NOT NULL,
	CONSTRAINT FK_SubCategory_Category 
	FOREIGN KEY (CategoryId) REFERENCES Category (Id)
)
