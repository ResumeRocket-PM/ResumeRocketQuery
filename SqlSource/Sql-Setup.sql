CREATE TABLE `account` (
  `AccountID` int NOT NULL AUTO_INCREMENT,
  `AccountAlias` varchar(36) NOT NULL,
  `AccountConfiguration` text NOT NULL,
  `UpdateDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `InsertDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`AccountID`)
)

CREATE TABLE `emailaddress` (
  `EmailAddressID` int NOT NULL AUTO_INCREMENT,
  `EmailAddress` varchar(300) NOT NULL,
  `AccountID` int DEFAULT NULL,
  `IsActive` bit(1) NOT NULL DEFAULT b'1',
  `UpdateDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `InsertDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`EmailAddressID`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci