using System.Security.Cryptography.X509Certificates;

namespace ResumeRocketQuery.DataLayer
{
    public static class DataLayerConstants
    {
        public static class StoredProcedures
        {
            public static class Account
            {
                public const string InsertAccount = @"
                    INSERT INTO Accounts (AccountAlias, FirstName, LastName, ProfilePhotoLink, Title, StateLocation, PortfolioLink)
                    VALUES (@accountAlias, @firstName, @lastName, @profilePhotoLink, @title, @stateLocation, @portfolioLink);
                    SELECT SCOPE_IDENTITY();";

                public const string SelectAccount = @"
                    SELECT AccountId, AccountAlias, FirstName, LastName, ProfilePhotoLink, Title, StateLocation, PortfolioLink
                    FROM Accounts
                    WHERE AccountId = @accountID;";

                public const string SelectAccountByFilter = @"
                    ";

                public const string UpdateAccount = @"
                    UPDATE Accounts
                    SET AccountAlias = @accountAlias,
                        FirstName = @firstName,
                        LastName = @lastName,
                        ProfilePhotoLink = @profilePhotoLink,
                        Title = @title,
                        StateLocation = @stateLocation,
                        PortfolioLink = @portfolioLink,
                        UpdateDate = GetDate()
                    WHERE AccountId = @accountId;";
            }

            public class Applications
            {
                public const string InsertApplication = @"

                    DECLARE @StatusID int;

                    SELECT @StatusID = StatusId
                    FROM ApplicationStatus
                    WHERE Status = @Status;

                    INSERT INTO Applications (AccountId, ApplyDate, StatusId, Position, CompanyName, JobPostingUrl, ResumeId, InsertDate, UpdateDate)
                    VALUES (@AccountId, @ApplyDate, @StatusId, @Position, @CompanyName, @JobPostingUrl, @ResumeId, GETDATE(), GETDATE());
                    SELECT SCOPE_IDENTITY();";

                public const string UpdateApplication = @"
                    DECLARE @StatusID int;

                    SELECT @StatusID = StatusId
                    FROM ApplicationStatus
                    WHERE Status = @Status;

                    IF @StatusID IS NULL
                    BEGIN
                        INSERT INTO ApplicationStatus (Status)
                        VALUES (@Status);

                        SET @StatusID = SCOPE_IDENTITY();
                    END

                    UPDATE Applications
                    SET StatusId = @StatusID,
                        UpdateDate = GETDATE()
                    WHERE ApplicationId = @ApplicationId;";

                public const string SelectApplicationByAccount = @"
                    SELECT ApplicationId, AccountId, ApplyDate, s.Status, Position, CompanyName, JobPostingUrl, ResumeId
                    FROM Applications a
                    JOIN ApplicationStatus s on a.StatusId = s.StatusId
                    WHERE AccountId = @AccountId;";

                public const string SelectApplication = @"
                    SELECT ApplicationId, AccountId, ApplyDate, s.Status, Position, CompanyName, JobPostingUrl, ResumeId
                    FROM Applications a
                    JOIN ApplicationStatus s on a.StatusId = s.StatusId
                    WHERE ApplicationId = @ApplicationId;";
            }

            public class EmailAddress
            {
                public const string InsertEmailAddress = @"
                    INSERT INTO EmailAddresses (EmailAddress, AccountId)
                    VALUES (@EmailAddress, @AccountId);
                    SELECT SCOPE_IDENTITY();";

                public const string UpdateEmailAddress = @"
                    UPDATE EmailAddresses
                    SET EmailAddress = @EmailAddress
                    WHERE AccountId = @AccountId;";

                public const string SelectEmailAddress = @"
                    SELECT EmailAddressId, EmailAddress, AccountId
                    FROM EmailAddresses
                    WHERE AccountId = @AccountId;";


                public const string SelectAccountByEmailAddress = @"
                    SELECT EmailAddressId, EmailAddress, AccountId
                    FROM EmailAddresses
                    WHERE EmailAddress = @EmailAddress;";
            }

            public class Logins
            {
                public const string InsertLogin = @"
                    INSERT INTO Logins (AccountId, Salt, Hash)
                    VALUES (@AccountId, @Salt, @Hash);
                    SELECT SCOPE_IDENTITY();";

                public const string UpdateLogin = @"
                    UPDATE Logins
                    SET Salt = @Salt,
                        Hash = @Hash
                    WHERE AccountId = @AccountId;";

                public const string SelectLogin = @"
                    SELECT LoginId, AccountId, Salt, Hash
                    FROM Logins
                    WHERE AccountId = @AccountId;";
            }


            public class Education
            {
                public const string InsertEducation = @"
                    INSERT INTO Educations (AccountId, SchoolName, Degree, Major, Minor, GraduationDate)
                    VALUES (@AccountId, @SchoolName, @Degree, @Major, @Minor, @GraduationDate);
                    SELECT SCOPE_IDENTITY();";

                public const string UpdateEducation = @"
                    UPDATE Educations
                    SET SchoolName = @SchoolName,
                        Degree = @Degree,
                        Major = @Major,
                        Minor = @Minor,
                        GraduationDate = @GraduationDate
                    WHERE EducationId = @EducationId;";

                public const string SelectEducation = @"
                    SELECT EducationId, AccountId, SchoolName, Degree, Major, Minor, GraduationDate
                    FROM Educations
                    WHERE AccountId = @AccountId;";

                public const string DeleteEducation = @"
                    DELETE FROM Educations
                    WHERE EducationId = @EducationId;";


                public const string DeleteEducationByAccount = @"
                    DELETE FROM Educations
                    WHERE AccountId = @AccountId;";
            }

            public class Portfolio
            {
                public const string InsertPortfolio = @"
                    INSERT INTO Portfolio (AccountId, Configuration)
                    VALUES (@AccountId, @Configuration);
                    SELECT SCOPE_IDENTITY();";


                public const string UpdatePortfolio = @"
                    UPDATE Portfolio
                    SET Configuration = @Configuration,
                        UpdateDate = GetDate()
                    WHERE PortfolioId = @PortfolioId";


                // Select
                public const string SelectPortfolio = @"
                    SELECT PortfolioId, AccountId, Configuration
                    FROM Portfolio
                    WHERE AccountId = @AccountId";

            }

            public class Resume
            {
                public const string InsertResume = @"
                    INSERT INTO Resumes (AccountId, Resume)
                    VALUES (@AccountId, CONVERT(VARBINARY(max),@Resume));
                    SELECT SCOPE_IDENTITY();";

                public const string UpdateResume = @"
                    UPDATE Resumes
                    SET Resume = CONVERT(VARBINARY(max),@Resume)
                    WHERE ResumeId = @ResumeId;";

                public const string SelectResume = @"
                    SELECT ResumeId, AccountId, CONVERT(nvarchar(max),Resume) as Resume
                    FROM Resumes
                    WHERE ResumeId = @ResumeId;";

                public const string SelectResumeByAccount = @"
                    SELECT ResumeId, AccountId, CONVERT(nvarchar(max),Resume) as Resume
                    FROM Resumes
                    WHERE AccountId = @AccountId;";

                public const string DeleteResume = @"
                    DELETE FROM Resumes
                    WHERE ResumeId = @ResumeId;";
            }

            public class Experience
            {
                public const string InsertExperience = @"
                    INSERT INTO Experience (AccountId, Company, Position, Type, Description, StartDate, EndDate)
                    VALUES (@AccountId, @Company, @Position, @Type, @Description, @StartDate, @EndDate);
                    SELECT SCOPE_IDENTITY();";

                public const string UpdateExperience = @"
                    UPDATE Experience
                    SET AccountId = @AccountId,
                        Company = @Company,
                        Position = @Position,
                        Type = @Type,
                        Description = @Description,
                        StartDate = @StartDate,
                        EndDate = @EndDate
                    WHERE ExperienceId = @ExperienceId;";

                public const string SelectExperience = @"
                    SELECT ExperienceId, AccountId, Company, Position, Type, Description, StartDate, EndDate
                    FROM Experience
                    WHERE AccountId = @AccountId;";

                public const string DeleteExperience = @"
                    DELETE FROM Experience
                    WHERE ExperienceId = @ExperienceId;";

                public const string DeleteExperienceByAccountId = @"
                    DELETE FROM Experience
                    WHERE AccountId = @AccountId;";
            }

            public class Skills
            {
                public const string InsertSkill = @"
                    INSERT INTO Skills (AccountId, Description)
                    VALUES (@AccountId, @Description);
                    SELECT SCOPE_IDENTITY();";

                public const string SelectSkill = @"
                    SELECT SkillId, AccountId, Description
                    FROM Skills
                    WHERE AccountId = @AccountId;";

                public const string DeleteSkill = @"
                    DELETE FROM Skills
                    WHERE SkillId = @SkillId;";

                public const string DeleteSkillByAccountId = @"
                    DELETE FROM Skills
                    WHERE AccountId = @AccountId;";
            }

            public class Search
            {
                public const string SearchFullTextIndex = @"
                    Select Top(@ResultCount) FullTextScore + SoundexScore as Score, * from (

	                    SELECT *,
		                       CASE
			                       WHEN FREETEXT((FirstName, LastName, StateLocation, Title), @SearchTerm) THEN 1
			                       ELSE 0
		                       END AS FullTextScore,
		                       CASE
			                       WHEN SOUNDEX(FirstName) = SOUNDEX(@SearchTerm) 
					                    OR SOUNDEX(LastName) = SOUNDEX(@SearchTerm) THEN 1
			                       ELSE 0
		                       END AS SoundexScore
	                    FROM dbo.Accounts
	                    WHERE FREETEXT((FirstName, LastName, StateLocation, Title), @SearchTerm)
	                       OR SOUNDEX(FirstName) = SOUNDEX(@SearchTerm)
	                       OR SOUNDEX(LastName) = SOUNDEX(@SearchTerm)
                    ) as s 

                    ORDER BY 1 DESC, 
	                    CASE WHEN ProfilePhotoLink IS NULL THEN 1 ELSE 0 END,
	                    CASE WHEN FirstName IS NULL OR LastName is null THEN 1 ELSE 0 END";

            }
        }
    }
}
