/*======================================================*/
/*  Created in April 2021                               */
/*  WEB 2021 April Semester					            */
/*  Diploma in IT/FI                                    */
/*                                                      */
/*  Database Script for setting up the database         */
/*  required for WEB Assignment.                        */
/*======================================================*/

Create Database CJP_DB
GO

Use CJP_DB
GO

/***************************************************************/
/***           Delete tables before creating                 ***/
/***************************************************************/

/* Table: dbo.Comment */
if exists (select * from sysobjects 
  where id = object_id('dbo.Comment') and sysstat & 0xf = 3)
  drop table dbo.Comment
GO

/* Table: dbo.CompetitionScore */
if exists (select * from sysobjects 
  where id = object_id('dbo.CompetitionScore') and sysstat & 0xf = 3)
  drop table dbo.CompetitionScore
GO

/* Table: dbo.Criteria */
if exists (select * from sysobjects 
  where id = object_id('dbo.Criteria') and sysstat & 0xf = 3)
  drop table dbo.Criteria
GO

/* Table: dbo.CompetitionSubmission */
if exists (select * from sysobjects 
  where id = object_id('dbo.CompetitionSubmission') and sysstat & 0xf = 3)
  drop table dbo.CompetitionSubmission
GO

/* Table: dbo.CompetitionJudge */
if exists (select * from sysobjects 
  where id = object_id('dbo.CompetitionJudge') and sysstat & 0xf = 3)
  drop table dbo.CompetitionJudge
GO

/* Table: dbo.Judge */
if exists (select * from sysobjects 
  where id = object_id('dbo.Judge') and sysstat & 0xf = 3)
  drop table dbo.Judge
GO

/* Table: dbo.Competitor */
if exists (select * from sysobjects 
  where id = object_id('dbo.Competitor') and sysstat & 0xf = 3)
  drop table dbo.Competitor
GO

/* Table: dbo.Competition */
if exists (select * from sysobjects 
  where id = object_id('dbo.Competition') and sysstat & 0xf = 3)
  drop table dbo.Competition
GO

/* Table: dbo.AreaInterest */
if exists (select * from sysobjects 
  where id = object_id('dbo.AreaInterest') and sysstat & 0xf = 3)
  drop table dbo.AreaInterest
GO


/***************************************************************/
/***                     Creating tables                     ***/
/***************************************************************/

/* Table: dbo.AreaInterest */
CREATE TABLE dbo.AreaInterest
(
  AreaInterestID 		int IDENTITY (1,1),
  [Name]				varchar(50) 	NOT NULL,
  CONSTRAINT PK_AreaInterest PRIMARY KEY NONCLUSTERED (AreaInterestID)
)
GO

/* Table: dbo.Competition */
CREATE TABLE dbo.Competition
(
  CompetitionID 		int IDENTITY (1,1),
  AreaInterestID 		int 	        NOT NULL,
  CompetitionName		varchar(255) 	NOT NULL,
  StartDate				datetime		NULL,
  EndDate				datetime		NULL,
  ResultReleasedDate	datetime		NULL,
  CONSTRAINT PK_Competition PRIMARY KEY NONCLUSTERED (CompetitionID),
  CONSTRAINT FK_Competition_AreaInterestID FOREIGN KEY (AreaInterestID) 
  REFERENCES dbo.AreaInterest(AreaInterestID),
)
GO

/* Table: dbo.Competitor */
CREATE TABLE dbo.Competitor
(
  CompetitorID 			int IDENTITY (1,1),
  CompetitorName		varchar(50) 	NOT NULL,
  Salutation			varchar(5)  	NULL	
                                        CHECK (Salutation IN ('Dr','Mr','Ms','Mrs','Mdm')),
  EmailAddr		    	varchar(50)  	NOT NULL,
  [Password]		    varchar(255)  	NOT NULL,
  CONSTRAINT PK_Competitor PRIMARY KEY NONCLUSTERED (CompetitorID)
)
GO

/* Table: dbo.Judge */
CREATE TABLE dbo.Judge
(
  JudgeID 				int IDENTITY (1,1),
  JudgeName				varchar(50) 	NOT NULL,
  Salutation			varchar(5)  	NULL	
                                        CHECK (Salutation IN ('Dr','Mr','Ms','Mrs','Mdm')),
  AreaInterestID 		int 	        NOT NULL,
  EmailAddr		    	varchar(50)  	NOT NULL,
  [Password]		    varchar(255)  	NOT NULL DEFAULT ('p@55Judge'),
  CONSTRAINT PK_Judge PRIMARY KEY NONCLUSTERED (JudgeID),
  CONSTRAINT FK_Judge_AreaInterestID FOREIGN KEY (AreaInterestID) 
  REFERENCES dbo.AreaInterest(AreaInterestID)
)
GO

/* Table: dbo.CompetitionJudge */
CREATE TABLE dbo.CompetitionJudge
(
  CompetitionID			int  			NOT NULL,
  JudgeID				int				NOT NULL,
  CONSTRAINT FK_CompetitionJudge_CompetitionID FOREIGN KEY (CompetitionID) 
  REFERENCES dbo.Competition(CompetitionID),
  CONSTRAINT FK_CompetitionJudge_JudgeID  FOREIGN KEY (JudgeID) 
  REFERENCES dbo.Judge(JudgeID)
)
GO

/* Table: dbo.CompetitionSubmission */
CREATE TABLE dbo.CompetitionSubmission
(
  CompetitionID			int  			NOT NULL,
  CompetitorID			int  			NOT NULL,
  FileSubmitted			varchar(255) 	NULL,
  DateTimeFileUpload	datetime		NULL,
  Appeal				varchar(255) 	NULL,
  VoteCount				int				NOT NULL,
  Ranking				int				NULL,
  CONSTRAINT FK_CompetitionSubmission_CompetitionID FOREIGN KEY (CompetitionID) 
  REFERENCES dbo.Competition(CompetitionID),
  CONSTRAINT FK_CompetitionSubmission_CompetitorID  FOREIGN KEY (CompetitorID) 
  REFERENCES dbo.Competitor(CompetitorID)
)
GO

/* Table: dbo.Criteria */
CREATE TABLE dbo.Criteria
(
  CriteriaID 			int IDENTITY (1,1),
  CompetitionID			int 	        NOT NULL,
  CriteriaName			varchar(50) 	NOT NULL,
  Weightage				int				NOT NULL,
  CONSTRAINT PK_Criteria PRIMARY KEY NONCLUSTERED (CriteriaID),
  CONSTRAINT FK_Criteria_CompetitionID FOREIGN KEY (CompetitionID) 
  REFERENCES dbo.Competition(CompetitionID)
)
GO

/* Table: dbo.CompetitionScore */
CREATE TABLE dbo.CompetitionScore
(
  CriteriaID			int  			NOT NULL,
  CompetitorID			int				NOT NULL,
  CompetitionID			int				NOT NULL,
  Score					int				NOT NULL,
  CONSTRAINT FK_CompetitionScore_CriteriaID FOREIGN KEY (CriteriaID) 
  REFERENCES dbo.Criteria(CriteriaID),
  CONSTRAINT FK_CompetitionScore_CompetitorID FOREIGN KEY (CompetitorID) 
  REFERENCES dbo.Competitor(CompetitorID),
  CONSTRAINT FK_CompetitionScore_CompetitionID  FOREIGN KEY (CompetitionID) 
  REFERENCES dbo.Competition(CompetitionID)
)
GO

/* Table: dbo.Comment  */
CREATE TABLE dbo.Comment 
(
  CommentID				int IDENTITY (1,1),
  CompetitionID			int  			NOT NULL,
  Description			varchar(255) 	NULL,
  DateTimePosted		datetime		NOT NULL DEFAULT (getdate()),
  CONSTRAINT PK_Comment PRIMARY KEY NONCLUSTERED (CommentID),
  CONSTRAINT FK_Comment_CompetitionID FOREIGN KEY (CompetitionID) 
  REFERENCES dbo.Competition(CompetitionID)
)
GO


/***************************************************************/
/***                Populate Sample Data                     ***/
/***************************************************************/
SET IDENTITY_INSERT [dbo].[AreaInterest] ON 
INSERT [dbo].[AreaInterest] ([AreaInterestID], [Name]) 
VALUES (1, 'Computer Programming')
INSERT [dbo].[AreaInterest] ([AreaInterestID], [Name]) 
VALUES (2, 'Singing')
INSERT [dbo].[AreaInterest] ([AreaInterestID], [Name]) 
VALUES (3, 'Dancing')
INSERT [dbo].[AreaInterest] ([AreaInterestID], [Name]) 
VALUES (4, 'Writing')
SET IDENTITY_INSERT [dbo].[AreaInterest] OFF

SET IDENTITY_INSERT [dbo].[Competition] ON 
INSERT [dbo].[Competition] ([CompetitionID], [AreaInterestID], [CompetitionName], [StartDate], [EndDate], [ResultReleasedDate]) 
VALUES (1, 1, 'LCU Hackathon 2020', '5-Oct-2020', '9-Oct-2020', '16-Oct-2020')
INSERT [dbo].[Competition] ([CompetitionID], [AreaInterestID], [CompetitionName], [StartDate], [EndDate], [ResultReleasedDate]) 
VALUES (2, 1, 'LCU Coding Challenge 2021', '4-Oct-2021', '6-Oct-2021', '15-Oct-2021')
INSERT [dbo].[Competition] ([CompetitionID], [AreaInterestID], [CompetitionName], [StartDate], [EndDate], [ResultReleasedDate]) 
VALUES (3, 2, 'SingForSG 2021', '8-Nov-2021', '12-Nov-2021', '19-Nov-2021')
INSERT [dbo].[Competition] ([CompetitionID], [AreaInterestID], [CompetitionName], [StartDate], [EndDate], [ResultReleasedDate]) 
VALUES (4, 4, 'Singapore Story Writing 2021', '6-Dec-2021', '10-Dec-2021', '17-Dec-2021')
SET IDENTITY_INSERT [dbo].[Competition] OFF

SET IDENTITY_INSERT [dbo].[Competitor] ON 
INSERT [dbo].[Competitor] ([CompetitorID], [CompetitorName], [Salutation], [EmailAddr], [Password]) 
VALUES (1, 'Peter Ghim', 'Mr', 'pg1@hotmail.com', 'p@55PG')
INSERT [dbo].[Competitor] ([CompetitorID], [CompetitorName], [Salutation], [EmailAddr], [Password]) 
VALUES (2, 'Fatimah Bte Ahmad', 'Ms', 'Fatimah2@gmail.com', 'p@55Fa')
INSERT [dbo].[Competitor] ([CompetitorID], [CompetitorName], [Salutation], [EmailAddr], [Password]) 
VALUES (3, 'Benjamin Bean', 'Mr', 'Bean3@yahoo.com', 'p@55Bean')
INSERT [dbo].[Competitor] ([CompetitorID], [CompetitorName], [Salutation], [EmailAddr], [Password]) 
VALUES (4, 'K Kannan', 'Dr', 'Kannan4@gmail.com', 'p@55Kannan')
INSERT [dbo].[Competitor] ([CompetitorID], [CompetitorName], [Salutation], [EmailAddr], [Password]) 
VALUES (5, 'Eliza Wong', 'Mdm', 'Eliza5@yahoo.com', 'p@55EWong')
INSERT [dbo].[Competitor] ([CompetitorID], [CompetitorName], [Salutation], [EmailAddr], [Password]) 
VALUES (6, 'Amy Ng-Tan', 'Mrs', 'Amy6@yahoo.com', 'p@55Amy')
INSERT [dbo].[Competitor] ([CompetitorID], [CompetitorName], [Salutation], [EmailAddr], [Password]) 
VALUES (7, 'Melvin Gould', 'Mr', 'MG7@hotmail.com', 'p@55MG')
INSERT [dbo].[Competitor] ([CompetitorID], [CompetitorName], [Salutation], [EmailAddr], [Password]) 
VALUES (8, 'Lisa Lee', 'Ms', 'Lisa8@yahoo.com', 'p@55Lisa')
INSERT [dbo].[Competitor] ([CompetitorID], [CompetitorName], [Salutation], [EmailAddr], [Password]) 
VALUES (9, 'John Tan', 'Mr', 'JT9@gmail.com', 'p@55John')
SET IDENTITY_INSERT [dbo].[Competitor] OFF

SET IDENTITY_INSERT [dbo].[Judge] ON 
INSERT [dbo].[Judge] ([JudgeID], [JudgeName], [Salutation], [AreaInterestID], [EmailAddr], [Password]) 
VALUES (1, 'Alice Avin', 'Ms', 2, 'Staff1@lcu.edu.sg', 'p@55Judge')
INSERT [dbo].[Judge] ([JudgeID], [JudgeName], [Salutation], [AreaInterestID], [EmailAddr], [Password]) 
VALUES (2, 'Ali Imran', 'Dr', 1, 'Staff2@lcu.edu.sg', 'p@55Judge')
INSERT [dbo].[Judge] ([JudgeID], [JudgeName], [Salutation], [AreaInterestID], [EmailAddr], [Password]) 
VALUES (3, 'Howard Johnson', 'Mr', 3, 'Staff3@lcu.edu.sg', 'p@55Judge')
INSERT [dbo].[Judge] ([JudgeID], [JudgeName], [Salutation], [AreaInterestID], [EmailAddr], [Password]) 
VALUES (4, 'Edward Lee', 'Mr', 4, 'Staff4@lcu.edu.sg', 'p@55Judge')
INSERT [dbo].[Judge] ([JudgeID], [JudgeName], [Salutation], [AreaInterestID], [EmailAddr], [Password]) 
VALUES (5, 'Xu Yazhi', 'Dr', 1, 'Staff5@lcu.edu.sg', 'p@55Judge')
INSERT [dbo].[Judge] ([JudgeID], [JudgeName], [Salutation], [AreaInterestID], [EmailAddr], [Password]) 
VALUES (6, 'Phobe Pander', 'Mdm', 3, 'Staff6@lcu.edu.sg', 'p@55Judge')
INSERT [dbo].[Judge] ([JudgeID], [JudgeName], [Salutation], [AreaInterestID], [EmailAddr], [Password]) 
VALUES (7, 'Jane Greenspan', 'Dr', 4, 'Staff7@lcu.edu.sg', 'p@55Judge')
INSERT [dbo].[Judge] ([JudgeID], [JudgeName], [Salutation], [AreaInterestID], [EmailAddr], [Password]) 
VALUES (8, 'Ang Bee Chin', 'Ms', 2, 'Staff8@lcu.edu.sg', 'p@55Judge')
INSERT [dbo].[Judge] ([JudgeID], [JudgeName], [Salutation], [AreaInterestID], [EmailAddr], [Password]) 
VALUES (9, 'Geetha', 'Dr', 1, 'Staff9@lcu.edu.sg', 'p@55Judge')
SET IDENTITY_INSERT [dbo].[Judge] OFF

INSERT [dbo].[CompetitionJudge] ([CompetitionID], [JudgeID]) VALUES (1,2)
INSERT [dbo].[CompetitionJudge] ([CompetitionID], [JudgeID]) VALUES (1,5)
INSERT [dbo].[CompetitionJudge] ([CompetitionID], [JudgeID]) VALUES (1,9)
INSERT [dbo].[CompetitionJudge] ([CompetitionID], [JudgeID]) VALUES (2,2)
INSERT [dbo].[CompetitionJudge] ([CompetitionID], [JudgeID]) VALUES (2,5)
INSERT [dbo].[CompetitionJudge] ([CompetitionID], [JudgeID]) VALUES (3,1)
INSERT [dbo].[CompetitionJudge] ([CompetitionID], [JudgeID]) VALUES (3,8)

INSERT [dbo].[CompetitionSubmission] ([CompetitionID], [CompetitorID], [FileSubmitted], [DateTimeFileUpload], [Appeal], [VoteCount], [Ranking]) 
VALUES (1,1,'File_1_1.pdf', '2020-10-08 15:00:00', NULL, 10, 2)
INSERT [dbo].[CompetitionSubmission] ([CompetitionID], [CompetitorID], [FileSubmitted], [DateTimeFileUpload], [Appeal], [VoteCount], [Ranking]) 
VALUES (1,3,'File_3_1.pdf', '2020-10-08 17:00:00', NULL, 7, 1)
INSERT [dbo].[CompetitionSubmission] ([CompetitionID], [CompetitorID], [FileSubmitted], [DateTimeFileUpload], [Appeal], [VoteCount], [Ranking]) 
VALUES (1,5,'File_5_1.pdf', '2020-10-09 09:00:00', 'The repetition of code in functionA() is to improve system security', 8, 3)
INSERT [dbo].[CompetitionSubmission] ([CompetitionID], [CompetitorID], [FileSubmitted], [DateTimeFileUpload], [Appeal], [VoteCount], [Ranking]) 
VALUES (1,8,'File_8_1.pdf', '2020-10-09 13:00:00', NULL, 2, 4)
INSERT [dbo].[CompetitionSubmission] ([CompetitionID], [CompetitorID], [FileSubmitted], [DateTimeFileUpload], [Appeal], [VoteCount], [Ranking]) 
VALUES (2,3, NULL, NULL, NULL, 0, NULL)
INSERT [dbo].[CompetitionSubmission] ([CompetitionID], [CompetitorID], [FileSubmitted], [DateTimeFileUpload], [Appeal], [VoteCount], [Ranking]) 
VALUES (2,1, NULL, NULL, NULL, 0, NULL)
INSERT [dbo].[CompetitionSubmission] ([CompetitionID], [CompetitorID], [FileSubmitted], [DateTimeFileUpload], [Appeal], [VoteCount], [Ranking]) 
VALUES (3,3, NULL, NULL, NULL, 0, NULL)

SET IDENTITY_INSERT [dbo].[Criteria] ON 
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (1, 1, 'Meet Business Needs', 30)
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (2, 1, 'User Friendliness', 20)
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (3, 1, 'Application Robustness', 25)
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (4, 1, 'Code Efficiency', 25)
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (5, 2, 'Meet Business Needs', 30)
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (6, 2, 'User Friendliness', 25)
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (7, 2, 'Application Robustness', 25)
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (8, 2, 'Code Efficiency', 20)
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (9, 3, 'Diction', 30)
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (10, 3, 'Vocal Quality', 45)
INSERT [dbo].[Criteria] ([CriteriaID], [CompetitionID], [CriteriaName], [Weightage]) 
VALUES (11, 3, 'Rhythmic Interpretation', 25)
SET IDENTITY_INSERT [dbo].[Criteria] OFF

INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (1, 1, 1, 8)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (2, 1, 1, 7)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (3, 1, 1, 9)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (4, 1, 1, 8)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (1, 3, 1, 9)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (2, 3, 1, 9)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (3, 3, 1, 9)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (4, 3, 1, 9)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (1, 5, 1, 7)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (2, 5, 1, 7)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (3, 5, 1, 7)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (4, 5, 1, 6)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (1, 8, 1, 6)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (2, 8, 1, 6)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (3, 8, 1, 5)
INSERT [dbo].[CompetitionScore] ([CriteriaID], [CompetitorID], [CompetitionID], [Score]) 
VALUES (4, 8, 1, 6)

SET IDENTITY_INSERT [dbo].[Comment] ON
INSERT [dbo].[Comment] ([CommentID], [CompetitionID], [Description], [DateTimePosted]) 
VALUES (1, 1, 'The standard of competition was high, hope there will be more competitors next year.', '2020-10-20 10:08:17')
SET IDENTITY_INSERT [dbo].[Comment] OFF

