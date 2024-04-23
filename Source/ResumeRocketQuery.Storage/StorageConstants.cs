using System.Security.Cryptography.X509Certificates;

namespace ResumeRocketQuery.Storage
{
    public static class StorageConstants
    {
        public static class StoredProcedures
        {
            public static string InsertPortfolio = @"
				INSERT INTO Portfolio (
							AccountId,
							PortfolioAlias,
							PortfolioConfiguration,
							InsertDate,
							UpdateDate
						)
						VALUES 
						(
							@AccountId,
							@PortfolioAlias,
							@PortfolioConfiguration,
							Now(),
							Now()
						);

						SELECT LAST_INSERT_ID();";


            public static string SelectPortfolio = @"
                SELECT 
					 PortfolioId
                    ,AccountID
			        ,PortfolioAlias
			        ,PortfolioConfiguration
		        FROM Portfolio
		        WHERE AccountID = @AccountID;";

            public static string InsertAccount = @"
				INSERT INTO Account (
							AccountAlias,
							AccountConfiguration,
							InsertDate,
							UpdateDate
						)
						VALUES 
						(
							@AccountAlias,
							@AccountConfiguration,
							Now(),
							Now()
						);

						SELECT LAST_INSERT_ID();";


            public static string SelectAccount = @"
                SELECT 
                     AccountID
			        ,AccountAlias
			        ,AccountConfiguration
		        FROM Account
		        WHERE AccountID = @AccountID;";


            public static string InsertEmailAddress = @"	
				INSERT INTO EmailAddress (
					EmailAddress,
					AccountID,
					IsActive,
					InsertDate,
					UpdateDate
				)
				VALUES 
				(
					@EmailAddress,
					@AccountID,
					1,
					Now(),
					Now()
				);
		
				SELECT LAST_INSERT_ID();";

            public static string SelectEmailAddress = @"
				SELECT EmailAddressID
						,EmailAddress
						,AccountID
				FROM EmailAddress
				WHERE IsActive = 1
				AND EmailAddressID = @EmailAddressID";

            public static string SelectEmailAddressByEmailAddress = @"
				SELECT EmailAddressID
					,EmailAddress
					,AccountID
				FROM EmailAddress
				WHERE IsActive = 1
				AND EmailAddress = @EmailAddress;";

            public static string SelectEmailAddressByAccountId = @"
				SELECT EmailAddressID
					,EmailAddress
					,AccountID
				FROM EmailAddress
				WHERE IsActive = 1
				AND AccountID = @AccountID;";

			// for Resumes Table

            public static string InsertResume = @"
				INSERT INTO resume (
				applyDate,
				jobUrl, 
				accountID,
				status,
				resume,
				position,
				companyName
				)
				VALUES 
				(
				@applyDate,
				@jobUrl, 
				@accountID,
				@status,
				@resume,
				@position,
				@companyName
				)";


            public static string SelectResume = @"
				select * 
				from resume 
				where accountID = @accountID";
				


        }
    }
}
