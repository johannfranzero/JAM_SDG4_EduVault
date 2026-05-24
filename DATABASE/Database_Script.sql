-- ============================================================
-- EduVault — FULL DATABASE SCRIPT
-- Includes: Schema (DDL) + Seed Data (DML)
-- Database  : EduVaultDB
-- SDG Target: UN SDG 4 — Quality Education
-- Version   : 1.0
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'EduVaultDB')
BEGIN
    CREATE DATABASE EduVaultDB;
    PRINT 'Database EduVaultDB created.';
END
GO

USE EduVaultDB;
GO

-- ============================================================
-- SECTION 1: SCHEMA (DDL)
-- ============================================================

-- Drop child tables first, then parent tables (respect FK dependency order)
IF OBJECT_ID('tblBookmarks',  'U') IS NOT NULL DROP TABLE tblBookmarks;
IF OBJECT_ID('tblAccessLog',  'U') IS NOT NULL DROP TABLE tblAccessLog;
IF OBJECT_ID('tblResources',  'U') IS NOT NULL DROP TABLE tblResources;
IF OBJECT_ID('tblCategories', 'U') IS NOT NULL DROP TABLE tblCategories;
IF OBJECT_ID('tblUsers',      'U') IS NOT NULL DROP TABLE tblUsers;
IF OBJECT_ID('tblLog',        'U') IS NOT NULL DROP TABLE tblLog;
GO

-- tblUsers
CREATE TABLE tblUsers (
    UserID       INT           IDENTITY(1,1) NOT NULL,
    Username     NVARCHAR(50)  NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    FullName     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(100) NULL,
    Role         NVARCHAR(20)  NOT NULL DEFAULT 'Student',
    IsActive     BIT           NOT NULL DEFAULT 1,
    DateCreated  DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_tblUsers  PRIMARY KEY (UserID),
    CONSTRAINT UQ_Username  UNIQUE (Username),
    CONSTRAINT CHK_Role     CHECK (Role IN ('Admin', 'Student'))
);
GO

-- tblCategories
CREATE TABLE tblCategories (
    CategoryID   INT           IDENTITY(1,1) NOT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    Description  NVARCHAR(500) NULL,
    IsActive     BIT           NOT NULL DEFAULT 1,
    DateCreated  DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_tblCategories PRIMARY KEY (CategoryID),
    CONSTRAINT UQ_CategoryName  UNIQUE (CategoryName)
);
GO

-- tblResources
CREATE TABLE tblResources (
    ResourceID     INT            IDENTITY(1,1) NOT NULL,
    Title          NVARCHAR(200)  NOT NULL,
    Description    NVARCHAR(1000) NULL,
    CategoryID     INT            NOT NULL,
    SubjectArea    NVARCHAR(100)  NULL,
    ResourceType   NVARCHAR(50)   NOT NULL,
    URL            NVARCHAR(500)  NULL,
    FilePath       NVARCHAR(500)  NULL,
    EducationLevel NVARCHAR(50)   NULL,
    Tags           NVARCHAR(500)  NULL,
    AddedBy        INT            NOT NULL,
    DateAdded      DATETIME       NOT NULL DEFAULT GETDATE(),
    IsActive       BIT            NOT NULL DEFAULT 1,
    ViewCount      INT            NOT NULL DEFAULT 0,
    CONSTRAINT PK_tblResources   PRIMARY KEY (ResourceID),
    CONSTRAINT FK_Res_Category   FOREIGN KEY (CategoryID) REFERENCES tblCategories(CategoryID),
    CONSTRAINT FK_Res_AddedBy    FOREIGN KEY (AddedBy)    REFERENCES tblUsers(UserID),
    CONSTRAINT CHK_ResourceType  CHECK (ResourceType IN ('E-Book','Video','Module','Reference','Article')),
    CONSTRAINT CHK_EduLevel      CHECK (EducationLevel IN ('Beginner','Intermediate','Advanced') OR EducationLevel IS NULL)
);
GO

-- tblAccessLog
CREATE TABLE tblAccessLog (
    LogID        INT          IDENTITY(1,1) NOT NULL,
    UserID       INT          NOT NULL,
    ResourceID   INT          NOT NULL,
    AccessDate   DATETIME     NOT NULL DEFAULT GETDATE(),
    AccessType   NVARCHAR(20) NOT NULL DEFAULT 'View',
    CONSTRAINT PK_tblAccessLog PRIMARY KEY (LogID),
    CONSTRAINT FK_Log_User     FOREIGN KEY (UserID)     REFERENCES tblUsers(UserID),
    CONSTRAINT FK_Log_Resource FOREIGN KEY (ResourceID) REFERENCES tblResources(ResourceID),
    CONSTRAINT CHK_AccessType  CHECK (AccessType IN ('View','Bookmark','Download'))
);
GO

-- tblLog
CREATE TABLE tblLog (
    LogID    INT            IDENTITY(1,1) NOT NULL,
    LogLevel NVARCHAR(10)   NOT NULL,
    Message  NVARCHAR(500)  NOT NULL,
    LogDate  DATETIME       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_tblLog PRIMARY KEY (LogID)
);
GO

-- tblBookmarks
CREATE TABLE tblBookmarks (
    BookmarkID     INT      IDENTITY(1,1) NOT NULL,
    UserID         INT      NOT NULL,
    ResourceID     INT      NOT NULL,
    DateBookmarked DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_tblBookmarks PRIMARY KEY (BookmarkID),
    CONSTRAINT FK_Bkm_User     FOREIGN KEY (UserID)     REFERENCES tblUsers(UserID),
    CONSTRAINT FK_Bkm_Resource FOREIGN KEY (ResourceID) REFERENCES tblResources(ResourceID),
    CONSTRAINT UQ_Bookmark     UNIQUE (UserID, ResourceID)
);
GO

-- Indexes
CREATE NONCLUSTERED INDEX IX_Resources_CategoryID  ON tblResources(CategoryID);
CREATE NONCLUSTERED INDEX IX_Resources_ResourceType ON tblResources(ResourceType);
CREATE NONCLUSTERED INDEX IX_Resources_DateAdded    ON tblResources(DateAdded DESC);
CREATE NONCLUSTERED INDEX IX_AccessLog_AccessDate   ON tblAccessLog(AccessDate DESC);
CREATE NONCLUSTERED INDEX IX_AccessLog_UserID       ON tblAccessLog(UserID);
GO

-- Views
IF OBJECT_ID('vwResourceSummary',      'V') IS NOT NULL DROP VIEW vwResourceSummary;
IF OBJECT_ID('vwMonthlyAccessSummary', 'V') IS NOT NULL DROP VIEW vwMonthlyAccessSummary;
GO
CREATE VIEW vwResourceSummary AS
    SELECT r.ResourceID, r.Title, r.Description, c.CategoryName,
           r.SubjectArea, r.ResourceType, r.URL, r.EducationLevel,
           r.Tags, u.FullName AS AddedByName, r.DateAdded, r.ViewCount, r.IsActive
    FROM   tblResources r
    INNER JOIN tblCategories c ON r.CategoryID = c.CategoryID
    INNER JOIN tblUsers       u ON r.AddedBy    = u.UserID;
GO
CREATE VIEW vwMonthlyAccessSummary AS
    SELECT YEAR(al.AccessDate) AS AccessYear, MONTH(al.AccessDate) AS AccessMonth,
           DATENAME(MONTH, al.AccessDate) AS MonthName,
           r.ResourceID, r.Title AS ResourceTitle, c.CategoryName,
           r.ResourceType, COUNT(al.LogID) AS TotalAccesses,
           COUNT(DISTINCT al.UserID) AS UniqueUsers
    FROM   tblAccessLog al
    INNER JOIN tblResources   r ON al.ResourceID = r.ResourceID
    INNER JOIN tblCategories  c ON r.CategoryID  = c.CategoryID
    GROUP BY YEAR(al.AccessDate), MONTH(al.AccessDate),
             DATENAME(MONTH, al.AccessDate), r.ResourceID, r.Title,
             c.CategoryName, r.ResourceType;
GO

PRINT '=== Schema created successfully ===';
GO

-- ============================================================
-- SECTION 2: SEED DATA (DML)
-- ============================================================
-- Passwords are SHA-256 hashed (uppercase hex) using AuthService.GenerateHash()
--   admin    password: Admin@123
--   student1 password: Student@123
-- ============================================================

-- Users
INSERT INTO tblUsers (Username, PasswordHash, FullName, Email, Role, IsActive)
VALUES ('admin',
        'E86F78A8A3CAF0B60D8E74E5942AA6D86DC150CD3C03338AEF25B7D2D7E3ACC7',
        'System Administrator', 'admin@eduvault.edu', 'Admin', 1);

INSERT INTO tblUsers (Username, PasswordHash, FullName, Email, Role, IsActive)
VALUES ('student1',
        'B2A1F4FD0A460606B34C8913E2981DAC8D2E283D778ABA586C416EE2629BFA54',
        'Juan dela Cruz', 'juan@student.edu', 'Student', 1);
GO

-- Categories
INSERT INTO tblCategories (CategoryName, Description) VALUES ('Mathematics',              'Algebra, Calculus, Statistics, and Discrete Math resources');
INSERT INTO tblCategories (CategoryName, Description) VALUES ('Science & Technology',     'Physics, Chemistry, Biology, and IT resources');
INSERT INTO tblCategories (CategoryName, Description) VALUES ('Programming',              'Software development, algorithms, and coding tutorials');
INSERT INTO tblCategories (CategoryName, Description) VALUES ('English & Communication',  'Academic writing, grammar, and communication skills');
INSERT INTO tblCategories (CategoryName, Description) VALUES ('Social Sciences',          'History, Economics, Political Science, and Sociology');
INSERT INTO tblCategories (CategoryName, Description) VALUES ('Health & Wellness',        'Physical education, mental health, and medical resources');
INSERT INTO tblCategories (CategoryName, Description) VALUES ('Business & Entrepreneurship', 'Management, marketing, and startup resources');
GO

-- Resources (using Admin UserID = 1)
DECLARE @AdminID   INT = 1;
DECLARE @ProgCatID INT = (SELECT CategoryID FROM tblCategories WHERE CategoryName = 'Programming');
DECLARE @MathCatID INT = (SELECT CategoryID FROM tblCategories WHERE CategoryName = 'Mathematics');
DECLARE @SciCatID  INT = (SELECT CategoryID FROM tblCategories WHERE CategoryName = 'Science & Technology');
DECLARE @EngCatID  INT = (SELECT CategoryID FROM tblCategories WHERE CategoryName = 'English & Communication');

INSERT INTO tblResources (Title, Description, CategoryID, SubjectArea, ResourceType, URL, EducationLevel, Tags, AddedBy) VALUES
('freeCodeCamp — Full Stack JavaScript', 'A comprehensive free curriculum covering HTML, CSS, JavaScript, React, Node.js, and databases. 300+ hours of free coursework.', @ProgCatID, 'Web Development', 'Video', 'https://www.freecodecamp.org/', 'Intermediate', 'javascript,web,fullstack,free', @AdminID);

INSERT INTO tblResources (Title, Description, CategoryID, SubjectArea, ResourceType, URL, EducationLevel, Tags, AddedBy) VALUES
('CS50x — Harvard Introduction to Computer Science', 'Harvard University''s world-renowned introduction to computer science. Free on edX.', @ProgCatID, 'Computer Science', 'Video', 'https://cs50.harvard.edu/x/', 'Beginner', 'cs50,harvard,python,c,algorithms,free', @AdminID);

INSERT INTO tblResources (Title, Description, CategoryID, SubjectArea, ResourceType, URL, EducationLevel, Tags, AddedBy) VALUES
('Khan Academy — Mathematics', 'Free mathematics instruction from arithmetic to calculus with practice exercises.', @MathCatID, 'General Mathematics', 'Video', 'https://www.khanacademy.org/math', 'Beginner', 'khan,math,free,calculus,algebra', @AdminID);

INSERT INTO tblResources (Title, Description, CategoryID, SubjectArea, ResourceType, URL, EducationLevel, Tags, AddedBy) VALUES
('MIT OpenCourseWare — Physics', 'Free lecture notes, problem sets, and exams from MIT''s physics courses.', @SciCatID, 'Physics', 'Module', 'https://ocw.mit.edu/courses/physics/', 'Advanced', 'MIT,physics,opencourseware,free,lectures', @AdminID);

INSERT INTO tblResources (Title, Description, CategoryID, SubjectArea, ResourceType, URL, EducationLevel, Tags, AddedBy) VALUES
('Purdue OWL — Academic Writing Guide', 'Free writing resources covering APA, MLA, Chicago styles, grammar, and academic writing.', @EngCatID, 'Academic Writing', 'Reference', 'https://owl.purdue.edu/owl/', 'Beginner', 'writing,grammar,APA,MLA,academic,free', @AdminID);
GO

-- Sample Access Logs (use dynamic lookups so IDs are correct after any re-run)
DECLARE @StudentID INT = (SELECT UserID     FROM tblUsers     WHERE Username    = 'student1');
DECLARE @Res1ID    INT = (SELECT ResourceID FROM tblResources WHERE Title        LIKE 'freeCodeCamp%');
DECLARE @Res2ID    INT = (SELECT ResourceID FROM tblResources WHERE Title        LIKE 'CS50x%');
DECLARE @Res3ID    INT = (SELECT ResourceID FROM tblResources WHERE Title        LIKE 'Khan Academy%');

INSERT INTO tblAccessLog (UserID, ResourceID, AccessDate, AccessType) VALUES (@StudentID, @Res1ID, DATEADD(DAY, -5,  GETDATE()), 'View');
INSERT INTO tblAccessLog (UserID, ResourceID, AccessDate, AccessType) VALUES (@StudentID, @Res1ID, DATEADD(DAY, -3,  GETDATE()), 'Bookmark');
INSERT INTO tblAccessLog (UserID, ResourceID, AccessDate, AccessType) VALUES (@StudentID, @Res2ID, DATEADD(DAY, -10, GETDATE()), 'View');
INSERT INTO tblAccessLog (UserID, ResourceID, AccessDate, AccessType) VALUES (@StudentID, @Res2ID, DATEADD(DAY, -2,  GETDATE()), 'View');
INSERT INTO tblAccessLog (UserID, ResourceID, AccessDate, AccessType) VALUES (@StudentID, @Res3ID, DATEADD(DAY, -15, GETDATE()), 'View');
INSERT INTO tblAccessLog (UserID, ResourceID, AccessDate, AccessType) VALUES (@StudentID, @Res3ID, DATEADD(DAY, -8,  GETDATE()), 'Bookmark');
INSERT INTO tblAccessLog (UserID, ResourceID, AccessDate, AccessType) VALUES (@StudentID, @Res1ID, DATEADD(MONTH, -1, GETDATE()), 'View');
INSERT INTO tblAccessLog (UserID, ResourceID, AccessDate, AccessType) VALUES (@StudentID, @Res3ID, DATEADD(MONTH, -1, GETDATE()), 'Download');
GO

PRINT '=== EduVault database setup complete ===';
GO
