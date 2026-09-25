/* =========================================
   PHONEBOOK DATABASE
========================================= */

IF DB_ID('PhonebookDB') IS NULL
BEGIN
    CREATE DATABASE PhonebookDB;
END
GO

USE PhonebookDB;
GO


/* =========================================
   CONTACTS TABLE
========================================= */

CREATE TABLE Contacts
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    Name NVARCHAR(255) NOT NULL,

    PhoneNumber NVARCHAR(50) NOT NULL UNIQUE,

    Email NVARCHAR(255) NULL,

    Address NVARCHAR(MAX) NULL,

    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* =========================================
   GET CONTACTS WITH PAGINATION AND SEARCH
========================================= */

CREATE PROCEDURE sp_GetContactsPaged
    @PageNumber INT,
    @PageSize INT,
    @SearchTerm NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        PhoneNumber,
        Email,
        Address,
        CreatedAt
    FROM Contacts
    WHERE
        @SearchTerm IS NULL
        OR @SearchTerm = ''
        OR Name LIKE '%' + @SearchTerm + '%'
        OR PhoneNumber LIKE '%' + @SearchTerm + '%'
        OR Email LIKE '%' + @SearchTerm + '%'
    ORDER BY Name ASC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM Contacts
    WHERE
        @SearchTerm IS NULL
        OR @SearchTerm = ''
        OR Name LIKE '%' + @SearchTerm + '%'
        OR PhoneNumber LIKE '%' + @SearchTerm + '%'
        OR Email LIKE '%' + @SearchTerm + '%';
END
GO

/* =========================================
   GET CONTACT BY ID
========================================= */

CREATE PROCEDURE sp_GetContactById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        PhoneNumber,
        Email,
        Address,
        CreatedAt
    FROM Contacts
    WHERE Id = @Id;
END
GO

/* =========================================
   INSERT CONTACT
========================================= */

CREATE PROCEDURE sp_InsertContact
    @Name NVARCHAR(255),
    @PhoneNumber NVARCHAR(50),
    @Email NVARCHAR(255) = NULL,
    @Address NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Contacts
    (
        Name,
        PhoneNumber,
        Email,
        Address
    )
    VALUES
    (
        @Name,
        @PhoneNumber,
        @Email,
        @Address
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

/* =========================================
   UPDATE CONTACT
========================================= */

CREATE PROCEDURE sp_UpdateContact
    @Id INT,
    @Name NVARCHAR(255),
    @PhoneNumber NVARCHAR(50),
    @Email NVARCHAR(255) = NULL,
    @Address NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Contacts
    SET
        Name = @Name,
        PhoneNumber = @PhoneNumber,
        Email = @Email,
        Address = @Address
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO


/* =========================================
   DELETE CONTACT
========================================= */

CREATE PROCEDURE sp_DeleteContact
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Contacts
    WHERE Id = @Id;

     SELECT @@ROWCOUNT AS RowsAffected;
END
GO

/* =========================================
   GET ALL CONTACTS FOR EXPORT
   ========================================= */

CREATE PROCEDURE sp_GetContactsForExport
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        PhoneNumber,
        Email,
        Address
    FROM Contacts
    ORDER BY Name ASC;
END
GO

