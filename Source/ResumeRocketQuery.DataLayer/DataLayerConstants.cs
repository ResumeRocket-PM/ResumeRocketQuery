using Microsoft.Identity.Client;
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
                    INSERT INTO Accounts (AccountAlias, FirstName, LastName, ProfilePhotoLink, Title, StateLocation, PortfolioLink, PrimaryResumeId)
                    VALUES (@accountAlias, @firstName, @lastName, @profilePhotoLink, @title, @stateLocation, @portfolioLink, @primaryResumeId);
                    SELECT SCOPE_IDENTITY();";

                public const string SelectAccount = @"
                    SELECT AccountId, AccountAlias, FirstName, LastName, ProfilePhotoLink, Title, StateLocation, PortfolioLink, PrimaryResumeId
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
                        UpdateDate = GetDate(),
                        PrimaryResumeId = @primaryResumeId
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

                public const string SelectPortfolioByPortfolioId = @"
                    SELECT PortfolioId, AccountId, Configuration
                    FROM Portfolio
                    WHERE PortfolioId = @PortfolioId";

            }

            public class Jobs 
            {
                public const string InsertJob = @"
                    INSERT INTO Jobs (JobUrl, JobCompany, JobDescription)
                    VALUES (@Url, @Company, @Description);";

                public const string GetJob = @"
                    SELECT JobId, JobUrl, JobCompany, JobDescription
                    FROM Jobs
                    WHERE JobUrl = @Url;";
            }

            public class Resume
            {
                public const string InsertResume = @"
                    INSERT INTO Resumes (AccountId, Resume, OriginalResume, OriginalResumeId, Version, InsertDate, UpdateDate)
                    VALUES (@AccountId, CONVERT(VARBINARY(max),@Resume), @OriginalResume, @OriginalResumeId, @Version, @InsertDate, @UpdateDate);
                    SELECT SCOPE_IDENTITY();";

                public const string UpdateResume = @"
                    UPDATE Resumes
                    SET Resume = CONVERT(VARBINARY(max),@Resume)
                    WHERE ResumeId = @ResumeId;";

                public const string SelectResume = @"
                    SELECT ResumeId, AccountId, Version, CONVERT(nvarchar(max),Resume) as Resume, OriginalResumeId, Version, InsertDate, UpdateDate
                    FROM Resumes
                    WHERE ResumeId = @ResumeId;";

                public const string SelectResumeByAccount = @"
                    SELECT ResumeId, AccountId, Version, CONVERT(nvarchar(max),Resume) as Resume, OriginalResumeId, Version, InsertDate, UpdateDate
                    FROM Resumes
                    WHERE AccountId = @AccountId;";

                public const string SelectResumeByOriginal = @"
                    SELECT ResumeId, AccountId, Version, CONVERT(nvarchar(max),Resume) as Resume, OriginalResumeId, Version, InsertDate, UpdateDate
                    FROM Resumes
                    WHERE OriginalResumeId = @OriginalResumeId
                    ORDER BY Resumes.Version;";

                public const string DeleteResume = @"
                    DELETE FROM Resumes
                    WHERE ResumeId = @ResumeId;";

                public const string GetNumResumeVersionsByAccount = @"
                    SELECT COUNT(1)
                    FROM Resumes
                    WHERE OriginalResumeId = @OriginalResumeId AND AccountId = @AccountId;";


                public const string GetNumOriginalResumesByAccount = @"
                    SELECT COUNT(1)
                    FROM Resumes
                    WHERE OriginalResume = 1 AND AccountId = @AccountId;";


                public const string SelectResumeChanges = @"
                    SELECT 
                        ResumeChangeId,
                        ResumeId,
                        OriginalText,
                        ModifiedText,
                        ExplanationString,
                        Accepted,
                        HtmlID,
                        ApplicationId   
                    FROM 
                        ResumeChange
                    WHERE 
                        ResumeId = @ResumeId;";

                public const string SelectResumeSuggestionsByApplicationId = @"
                    SELECT 
                        ResumeChangeId,
                        ResumeId,
                        OriginalText,
                        ModifiedText,
                        ExplanationString,
                        Accepted,
                        ApplicationId   
                    FROM 
                        ResumeChange
                    WHERE 
                        ApplicationId = @ApplicationId;";

                public const string InsertResumeChanges = @"
                    INSERT INTO ResumeChange (
                        ResumeId, 
                        OriginalText, 
                        ModifiedText, 
                        ExplanationString, 
                        Accepted, 
                        HtmlID,
                        ApplicationId
                    ) VALUES (
                        @ResumeId, 
                        @OriginalText, 
                        @ModifiedText, 
                        @ExplanationString, 
                        @Accepted, 
                        @HtmlID,
                        @ApplicationId
                    );
                    SELECT SCOPE_IDENTITY();
                    ";

                public const string UpdateResumeChanges = @"
                    UPDATE ResumeChange
                    SET Accepted = @Accepted
                    WHERE ResumeChangeId = @ResumeChangeId;
                ";
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
            public class Profile
            {
                //search State query
                public const string SearchStatesNameASC = @"
                SELECT *
                FROM States
                WHERE StatesName LIKE @stateName
                ORDER BY StatesName ASC;
                ";

                public const string SearchStatesNameDESC = @"
                SELECT *
                FROM States
                WHERE StatesName LIKE @stateName
                ORDER BY StatesName DESC;
                ";

                //seach University query
                public const string SearchUniversityNameASC = @"
                SELECT *
                FROM University
                WHERE UniversityName LIKE @uName
                ORDER BY UniversityName ASC;
                ";

                public const string SearchUniversityNameDESC = @"
                SELECT *
                FROM dbo.University
                WHERE UniversityName LIKE @uName
                ORDER BY UniversityName DESC;
                ";

                //search career query
                public const string SearchCareerNameASC = @"
                SELECT *
                FROM Career
                WHERE CareerName LIKE @cName
                ORDER BY CareerName ASC
                ";

                public const string SearchCareerNameDESC = @"
                SELECT *
                FROM Career
                WHERE CareerName LIKE @cName
                ORDER BY CareerName DESC
                ";

                //search career query
                public const string SearchMajorNameASC = @" 
                SELECT *
                FROM Major
                WHERE MajorName LIKE @mName
                ORDER BY MajorName ASC";

                public const string SearchMajorNameDESC = @" 
                SELECT *
                FROM Major
                WHERE MajorName LIKE @mName
                ORDER BY MajorName DESC";


            }

            // ***** These are not usable yet, do not know if we're doing
            // an images table, or/and ImageId
            public class Image
            {
                public const string InsertImage = @"
                    INSERT INTO Images (ImageUrl, FileName)
                    VALUES (@imageUrl, @fileName);
                    SELECT SCOPE_IDENTITY();";

                public const string SelectImage = @"
                    SELECT ImageId, ImageUrl, FileName
                    FROM Images
                    WHERE ImageId = @imageId;";

                public const string SelectImageByFileName = @"
                    SELECT ImageId, ImageUrl, FileName
                    FROM Images
                    WHERE FileName = @fileName;";

                public const string DeleteImage = @"
                    DELETE FROM Images
                    WHERE ImageId = @imageId;";
            }

            public class Chat
            {
                public const string AddNewFriends = @"
                INSERT INTO Friendship (AccountId1, AccountId2, Status, CreatedTime)
                VALUES (@accountId1, @accountId2, @status1, CURRENT_TIMESTAMP);
                
                INSERT INTO Friendship (AccountId1, AccountId2, Status, CreatedTime)
                VALUES (@accountId2, @accountId1, @status2, CURRENT_TIMESTAMP);
                SELECT * FROM Friendship
                WHERE (AccountId1 = @accountId1 and AccountId2 = @accountId2)
                ";
                
                public const string GetFriendsByAccount = @"
                SELECT * FROM Friendship
                WHERE (AccountId1 = @accountId1 and AccountId2 = @accountId2)
                   OR (AccountId1 = @accountId2 and AccountId2 = @accountId1)";

                public const string UpdateFriendsStatus = @"
                UPDATE Friendship 
                SET Status = @status1 
                WHERE AccountId1 = @accountId1 AND AccountId2 = @accountId2;
                
                UPDATE Friendship 
                SET Status = @status2 
                WHERE AccountId1 = @accountId2 AND AccountId2 = @accountId1;
                SELECT * FROM Friendship
                WHERE (AccountId1 = @accountId1 and AccountId2 = @accountId2)
                ;
                ";

                public const string DeleteFriendsPair = @"
                DELETE FROM Friendship
                OUTPUT DELETED.AccountId1, DELETED.AccountId2
                WHERE (AccountId1 = @accountId1 AND AccountId2 = @accountId2)
                   OR (AccountId1 = @accountId2 AND AccountId2 = @accountId1);
                ";
                public const string FindAllFrinedsByAccountId = @"
                SELECT DISTINCT
                Accounts.AccountId,
                Friendship.FriendsId, 
                Accounts.FirstName, 
                Accounts.LastName, 
                Accounts.ProfilePhotoLink, 
                Accounts.PortfolioLink, 
                EmailAddresses.EmailAddress,
                Friendship.CreatedTime, 
                Friendship.Status
                FROM 
                    Accounts
                LEFT JOIN 
                    EmailAddresses ON Accounts.AccountId = EmailAddresses.AccountId
                JOIN 
                    Friendship ON (
                        (Accounts.AccountId = Friendship.AccountId2 AND Friendship.AccountId1 = @accountId AND Friendship.Status Like @status)
                    )
                WHERE 
                    Friendship.Status Like @status 
                    AND (
                        Friendship.AccountId1 = @accountId OR Friendship.AccountId2 = @accountId
                    );
                ";
                


                public const string SearchUserByName = @"
                SELECT DISTINCT 
                    a.AccountId, 
                    a.FirstName,
                    a.LastName,
                    a.ProfilePhotoLink,
                    a.PortfolioLink,
                    e.EmailAddress, 
                    f.Status
                FROM Accounts a
                    LEFT JOIN EmailAddresses e ON a.AccountId = e.AccountId
                    LEFT JOIN Friendship f ON (
                        (f.AccountId1 = a.AccountId AND f.AccountId2 = 7041) 
                        OR (f.AccountId1 = 7041 AND f.AccountId2 = a.AccountId)
                    )
                WHERE a.FirstName LIKE @fName OR a.LastName LIKE @lName
                ";
                
                public const string SearchUserByEmail = @"
                SELECT 
                    a.AccountId, 
                    a.FirstName,
                    a.LastName,
                    a.ProfilePhotoLink,
                    a.PortfolioLink,
                    e.EmailAddress, 
                    f.Status
                FROM Accounts a
                JOIN EmailAddresses e ON a.AccountId = e.AccountId
                LEFT JOIN Friendship f ON (
                    (f.AccountId1 = a.AccountId AND f.AccountId2 = @myId) 
                    OR (f.AccountId1 = @myId AND f.AccountId2 = a.AccountId)
                )
                WHERE e.EmailAddress LIKE @email
                ";

                public const string SearchUserByPortfolio = @"
                SELECT DISTINCT 
                    a.AccountId, 
                    a.FirstName,
                    a.LastName,
                    a.ProfilePhotoLink,
                    a.PortfolioLink,
                    e.EmailAddress, 
                    f.Status
                FROM Accounts a
                    LEFT JOIN EmailAddresses e ON a.AccountId = e.AccountId
                    LEFT JOIN Friendship f ON (
                        (f.AccountId1 = a.AccountId AND f.AccountId2 = 7041) 
                        OR (f.AccountId1 = 7041 AND f.AccountId2 = a.AccountId)
                    )
                WHERE a.PortfolioLink = @pLink
                ";

                public const string UpdateFriendStatus = @"
                UPDATE Friendship
                SET Status = @status, CreatedTime = CURRENT_TIMESTAMP
                OUTPUT INSERTED.*                
                WHERE (FriendsId = @friendsId);";


                public const string GetFriendPairs = @"
                SELECT * FROM Friendship 
                WHERE FriendsId = @friendsId";

                public const string DeleteFriend = @"
                DECLARE @accountId1 INT = @inputAccountId1;
                DECLARE @accountId2 INT = @inputAccountId2;

                IF @accountId1 > @accountId2
                BEGIN
                    DECLARE @Temp INT = @accountId1;
                    SET @accountId1 = @accountId2;
                    SET @accountId2 = @Temp;
                END                
                
                DELETE FROM Friendship
                WHERE AccountId1 = @accountId1 and AccountId2 = @accountId2;";
                //--------------------------------------------------MESSAGES--------------------------------------------------------
                
                public const string GetAlltalkedAccounts = @"
                WITH RankedResults AS (
                SELECT 
                    a.AccountId,                    
                    a.FirstName,
                    a.LastName,
                    a.ProfilePhotoLink,
                    e.EmailAddress,
                    m.LatestMsgTime AS MsgTime,
                    f.Status,
                    ROW_NUMBER() OVER (PARTITION BY a.AccountId ORDER BY m.LatestMsgTime DESC) AS RowNum
                FROM 
                    (SELECT 
                        CASE 
                            WHEN SendId = @accountId THEN ReceiveId 
                            ELSE SendId 
                        END AS RelatedAccountId,
                        MAX(MsgTime) AS LatestMsgTime
                     FROM 
                        Messages
                     WHERE 
                        SendId = @accountId OR ReceiveId = @accountId
                     GROUP BY 
                        CASE 
                            WHEN SendId = @accountId THEN ReceiveId 
                            ELSE SendId 
                        END
                    ) AS m
                JOIN 
                    Accounts AS a ON m.RelatedAccountId = a.AccountId
                LEFT JOIN 
                    EmailAddresses AS e ON a.AccountId = e.AccountId
                LEFT JOIN 
                    Friendship AS f ON 
                    (f.AccountId1 = @accountId AND f.AccountId2 = a.AccountId) 
                    OR (f.AccountId1 = a.AccountId AND f.AccountId2 = @accountId)
                )
                SELECT 
                    AccountId,
                    FirstName,
                    LastName,
                    ProfilePhotoLink,
                    EmailAddress,
                    MsgTime,
                    Status
                FROM 
                    RankedResults
                WHERE 
                    RowNum = 1
                ORDER BY 
                    MsgTime DESC;
                ";

                public const string AddMsgByFId = @"
                INSERT INTO Messages (FriendId, SendId, ReceiveId, MsgContent, MsgTime, MsgStatus)
                SELECT FriendsId, AccountId1, AccountId2, @newMsg, CURRENT_TIMESTAMP, 'sending'
                FROM Friendship
                WHERE FriendsId = @fId;
                SELECT TOP (100) * FROM Messages 
                WHERE FriendId = @fId 
                ORDER BY MsgTime DESC";

                public const string AddMsgByAId = @"
                INSERT INTO Messages (FriendId, SendId, ReceiveId, MsgContent, MsgTime, MsgStatus)
                VALUES (@fId, @sId, @rId, @content, CURRENT_TIMESTAMP, @status);
                SELECT TOP(100) * FROM Messages
                WHERE FriendId = @fId
                ORDER BY MsgTime DESC";

                public const string UpdateMsgStatus = @"
                UPDATE Messages
                SET MsgStatus = @status
                OUTPUT INSERTED.*                
                WHERE (MsgId = @msgId);";

                public const string DeleteMsg = @"
                DELETE FROM Messages
                WHERE MsgId = @msgId;
                ";
                public const string GetMsgbyMsgId = @"
                SELECT * From Messages
                WHERE MsgId = @msgId
                ";

                public const string GetAllMsgContent = @"
                SELECT * FROM (
                    SELECT TOP(200) * 
                    FROM Messages 
                    WHERE (SendId = @aId1 AND ReceiveId = @aId2) 
                       OR (SendId = @aId2 AND ReceiveId = @aId1)
                    ORDER BY MsgTime DESC
                ) AS LatestMessages
                ORDER BY MsgTime ASC;";

                public const string GetRecentMsgContent = @"
                SELECT TOP(@num) * FROM Messages WHERE FriendId = @fId";

                public const string AddMessageEntity = @"
                INSERT INTO Messages (SendId, ReceiveId, MsgContent, MsgStatus, MsgTime)
                VALUES(@sendId, @receiveId, @text, @status, CURRENT_TIMESTAMP)
                SELECT SCOPE_IDENTITY();
                ";

                public const string GetMessageIntityByFId = @"
                SELECT * FROM Messages
                WHERE FriendId = @friendId";

                public const string GetMessageEntityByAId = @"
                DECLARE @accountId1 INT = @inputAccountId1;
                DECLARE @accountId2 INT = @inputAccountId2;

                IF @accountId1 > @accountId2
                BEGIN
                    DECLARE @Temp INT = @accountId1;
                    SET @accountId1 = @accountId2;
                    SET @accountId2 = @Temp;
                END                 
                
                SELECT * FROM Messages
                WHERE AccountId1 = @accountId1 and AccountId2 = @accountId2";

                //public const string UpdateMessageContent = @"";

            }
        }
    }
}
