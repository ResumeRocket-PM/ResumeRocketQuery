CREATE Database [ResumeRocketQueryService]
GO

USE [ResumeRocketQueryService]
GO

CREATE SCHEMA [ResumeRocketQueryService] Authorization dbo;
GO

BEGIN TRANSACTION

	CREATE TABLE [ResumeRocketQueryService].[Account]
	(
		[AccountID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[AccountAlias] Varchar(36) NOT NULL,
		[AccountConfiguration] Varchar(max) NOT NULL,
		[UpdateDate] DateTime default GetDate() NOT NULL,
		[InsertDate] DateTime default GetDate() NOT NULL,
	)
	GO

	CREATE TABLE [ResumeRocketQueryService].[EmailAddress]
	(
		[EmailAddressID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[EmailAddress] varchar(300) NOT NULL,
		[AccountID] INT FOREIGN KEY REFERENCES [ResumeRocketQueryService].[Account](AccountID),
		[IsActive] bit default 1 NOT NULL,
		[UpdateDate] DateTime default GetDate() NOT NULL,
		[InsertDate] DateTime default GetDate() NOT NULL,
	)
	GO

COMMIT TRANSACTION

BEGIN TRANSACTION
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	CREATE PROCEDURE [ResumeRocketQueryService].[usp_SelectAccount] (
		@AccountID int
	)
	AS
	BEGIN

		SELECT [AccountID]
			,[AccountAlias]
			,[AccountConfiguration]
		FROM [ResumeRocketQueryService].[Account]
		WHERE AccountID = @AccountID

	END
	GO
COMMIT TRANSACTION

BEGIN TRANSACTION
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	CREATE PROCEDURE [ResumeRocketQueryService].[usp_InsertAccount] (
		@AccountAlias Varchar(36),
		@AccountConfiguration VARCHAR(MAX)
	)
	AS
	BEGIN

		INSERT INTO [ResumeRocketQueryService].[Account] (
			[AccountAlias],
			[AccountConfiguration],
			[InsertDate],
			[UpdateDate]
		)
		VALUES 
		(
			@AccountAlias,
			@AccountConfiguration,
			GETDATE(),
			GETDATE()
		)
		
		SELECT CAST(SCOPE_IDENTITY() AS INT) AS AccountID
	END
	GO
COMMIT TRANSACTION

BEGIN TRANSACTION
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	CREATE PROCEDURE [ResumeRocketQueryService].[usp_InsertEmailAddress] (
		@EmailAddress Varchar(300),
		@AccountID int
	)
	AS
	BEGIN

		INSERT INTO [ResumeRocketQueryService].[EmailAddress] (
			[EmailAddress],
			[AccountID],
			IsActive,
			[InsertDate],
			[UpdateDate]
		)
		VALUES 
		(
			@EmailAddress,
			@AccountID,
			1,
			GETDATE(),
			GETDATE()
		)
		
		SELECT CAST(SCOPE_IDENTITY() AS INT) AS EmailAddressID
	END
	GO
COMMIT TRANSACTION

 BEGIN TRANSACTION
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	CREATE PROCEDURE [ResumeRocketQueryService].[usp_SelectEmailAddress] (
		@EmailAddressID int
	)
	AS
	BEGIN

		SELECT [EmailAddressID]
			  ,[EmailAddress]
			  ,[AccountID]
		  FROM [ResumeRocketQueryService].[ResumeRocketQueryService].[EmailAddress]
		  WHERE IsActive = 1
		  AND EmailAddressID = @EmailAddressID
	END
	GO
COMMIT TRANSACTION

 BEGIN TRANSACTION
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	CREATE PROCEDURE [ResumeRocketQueryService].[usp_SelectEmailAddressByEmailAddress] (
		@EmailAddress VARCHAR(300)
	)
	AS
	BEGIN

		SELECT [EmailAddressID]
			  ,[EmailAddress]
			  ,[AccountID]
		  FROM [ResumeRocketQueryService].[ResumeRocketQueryService].[EmailAddress]
		  WHERE IsActive = 1
		  AND [EmailAddress] = @EmailAddress
	END
	GO
COMMIT TRANSACTION

 BEGIN TRANSACTION
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	CREATE PROCEDURE [ResumeRocketQueryService].[usp_SelectEmailAddressByAccountID] (
		@AccountID int
	)
	AS
	BEGIN

		SELECT [EmailAddressID]
			  ,[EmailAddress]
			  ,[AccountID]
		  FROM [ResumeRocketQueryService].[ResumeRocketQueryService].[EmailAddress]
		  WHERE IsActive = 1
		  AND [AccountID] = @AccountID
	END
	GO
COMMIT TRANSACTION
