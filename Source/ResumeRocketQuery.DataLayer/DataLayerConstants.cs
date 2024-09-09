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

            public class EmailAddress
            {
                public const string InsertEmailAddress = @"
                    INSERT INTO EmailAddress (EmailAddress, AccountId)
                    VALUES (@EmailAddress, @AccountId);
                    SELECT SCOPE_IDENTITY();";

                public const string UpdateEmailAddress = @"
                    UPDATE EmailAddress
                    SET EmailAddress = @EmailAddress,
                    WHERE AccountId = @AccountId;";

                public const string SelectEmailAddress = @"
                    SELECT EmailAddressId, EmailAddress, AccountId
                    FROM EmailAddress
                    WHERE AccountId = @AccountId;";


                public const string SelectAccountByEmailAddress = @"
                    SELECT EmailAddressId, EmailAddress, AccountId
                    FROM EmailAddress
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
        }
    }
}
