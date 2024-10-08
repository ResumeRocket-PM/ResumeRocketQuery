

Declare @SearchTerm varchar(100) = 'joh'

SELECT * FROM dbo.Accounts
WHERE CONTAINS((FirstName, LastName), @SearchTerm) 




Select * from sys.fulltext_indexes 
Select * from sys.fulltext_catalogs 

SELECT 
    FULLTEXTCATALOGPROPERTY('ResumeRocketCatalog', 'PopulateStatus') AS PopulateStatus;

DROP FULLTEXT CATALOG ResumeRocketCatalog
DROP FULLTEXT INDEX ON dbo.Accounts


CREATE FULLTEXT CATALOG ResumeRocketCatalog;

CREATE FULLTEXT INDEX ON dbo.Accounts
(
    FirstName LANGUAGE 1033,
    LastName LANGUAGE 1033,
	StateLocation Language 1033, 
	Title Language 1033
)
KEY INDEX [PK__Accounts__349DA5A6866A1D6D]
ON ResumeRocketCatalog
WITH CHANGE_TRACKING AUTO;

ALTER FULLTEXT INDEX ON dbo.Accounts START FULL POPULATION;



	