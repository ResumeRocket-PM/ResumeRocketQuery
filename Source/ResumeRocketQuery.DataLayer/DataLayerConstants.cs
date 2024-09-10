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
                    WHERE PortfolioId = @PortfolioId;";


                // Select
                public const string SelectPortfolio = @"
                    SELECT PortfolioId, AccountId, Configuration
                    FROM Portfolio
                    WHERE AccountId = @AccountId;";

            }

            public class Resume
            {
                public const string InsertResume = @"
                    INSERT INTO Resumes (AccountId, Resume)
                    VALUES (@AccountId, @Resume);
                    SELECT SCOPE_IDENTITY();";

                public const string UpdateResume = @"
                    UPDATE Resumes
                    SET Resume = @Resume
                    WHERE ResumeId = @ResumeId;";

                public const string SelectResume = @"
                    SELECT ResumeId, AccountId, Resume
                    FROM Resumes
                    WHERE ResumeId = @ResumeId;";

                public const string SelectResumeByAccount = @"
                    SELECT ResumeId, AccountId, Resume
                    FROM Resumes
                    WHERE AccountId = @AccountId;";

                public const string DeleteResume = @"
                    DELETE FROM Resumes
                    WHERE ResumeId = @ResumeId;";
            }
        }
    }
}
