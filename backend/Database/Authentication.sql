USE PhonebookDB;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(512) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetUserForLogin
    @Username NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Username,
        PasswordHash
    FROM dbo.Users
    WHERE Username = @Username;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertUserIfNotExists
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(512)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Users WITH (UPDLOCK, HOLDLOCK)
        WHERE Username = @Username
    )
    BEGIN
        INSERT INTO dbo.Users
        (
            Username,
            PasswordHash
        )
        VALUES
        (
            @Username,
            @PasswordHash
        );

        SELECT CAST(1 AS INT) AS WasCreated;
    END
    ELSE
    BEGIN
        SELECT CAST(0 AS INT) AS WasCreated;
    END
END
GO
