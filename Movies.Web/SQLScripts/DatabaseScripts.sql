
Use Master
Go

-- DROP Database Movies

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Movies')
BEGIN
    CREATE DATABASE [Movies]
END
GO

USE [Movies]
GO

-- DROP TABLE dbo.MovieSettings

IF OBJECT_ID('dbo.MovieSettings', 'U') IS NULL
	CREATE TABLE [dbo].[MovieSettings](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](255) NOT NULL,
		[Value] [varchar](255) NOT NULL,
		[Description] [varchar](255) NULL,
	 CONSTRAINT [PK_MovieSettings] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	GO

SELECT 'Before Insert', * FROM dbo.MovieSettings (NOLOCK)

IF NOT EXISTS (SELECT ID FROM dbo.MovieSettings (NOLOCK) WHERE Name = 'omdbApiUrl')
	INSERT INTO dbo.MovieSettings (Name, Value, Description) VALUES ('omdbApiUrl', 'http://www.omdbapi.com/', 'Open Movie Database API URL')

IF NOT EXISTS (SELECT ID FROM dbo.MovieSettings (NOLOCK) WHERE Name = 'apiKey')
	INSERT INTO MovieSettings (Name, Value, Description) VALUES ('apiKey', 'a43a3308', 'Encrypted API Key for security to access omdb api.')

IF NOT EXISTS (SELECT ID FROM dbo.MovieSettings (NOLOCK) WHERE Name = 'defaultPageSize')
	INSERT INTO MovieSettings (Name, Value, Description) VALUES ('defaultPageSize', '10', 'PageSize to display results')

SELECT 'After Insert', * FROM dbo.MovieSettings (NOLOCK)
GO

CREATE OR ALTER PROCEDURE [dbo].[ap_Movies_MovieSettings_Get]
AS
/*
	Description		:	Get all Movie settings
	Usage			:	
	Modifications	:
		10/16/2020	: Sridhar Balas	- Created
*/
BEGIN
	
	SELECT 
		[ID]
      ,[Name]
	  ,[Value]
	  ,[Description]
  FROM [Movies].[dbo].[MovieSettings] (NOLOCK)

END
GO
