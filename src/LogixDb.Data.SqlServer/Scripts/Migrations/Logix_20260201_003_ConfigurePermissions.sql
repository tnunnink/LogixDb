-- 1. Create Login for the Windows Virtual Account
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'NT SERVICE\LogixDb')
    BEGIN
        DECLARE @sql NVARCHAR(MAX) = N'CREATE LOGIN [NT SERVICE\LogixDb] FROM WINDOWS WITH DEFAULT_DATABASE = ' + QUOTENAME(DB_NAME());
        EXEC sp_executesql @sql;
    END
GO

-- 2. Create the Database User for the Service
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'LogixServiceUser')
    BEGIN
        CREATE USER [LogixServiceUser] FOR LOGIN [NT SERVICE\LogixDb];
    END
GO

-- 3. Grant Ingestion Permissions to the Service User
-- We make the service user the owner of our core schemas so it can manage tables and data
ALTER AUTHORIZATION ON SCHEMA::logix TO [LogixServiceUser];
ALTER AUTHORIZATION ON SCHEMA::qa TO [LogixServiceUser];

-- 4. Secure by Default: Restrict public (all other users) to Read & Execute
-- This prevents them from manually INSERT/UPDATE/DELETE on tables
GRANT SELECT, EXECUTE ON SCHEMA::logix TO [public];
GRANT SELECT, EXECUTE ON SCHEMA::qa TO [public];

-- 5. Special case: The service user needs to run the migration itself
-- If logixdb migrate is run as the service user, it needs db_owner during the migration window.
ALTER ROLE [db_owner] ADD MEMBER [LogixServiceUser];
GO