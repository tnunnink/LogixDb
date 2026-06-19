-- Check if we are running on Windows. 
-- sys.dm_os_host_info is available in SQL Server 2017+
DECLARE @isWindows BIT = 0;
IF EXISTS (SELECT 1 FROM sys.dm_os_host_info WHERE host_platform = 'Windows')
BEGIN
    SET @isWindows = 1;
END

IF @isWindows = 1
BEGIN
    -- 1. Create Login for the Virtual Account (Only on Windows)
    IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'NT SERVICE\LogixDb')
    BEGIN
        DECLARE @sql NVARCHAR(MAX);
        SET @sql = N'CREATE LOGIN [NT SERVICE\LogixDb] FROM WINDOWS WITH DEFAULT_DATABASE = ' + QUOTENAME(DB_NAME());
        EXEC sp_executesql @sql;
    END

    -- 2. Create User and Grant Roles
    IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'LogixServiceUser')
    BEGIN
        CREATE USER [LogixServiceUser] FOR LOGIN [NT SERVICE\LogixDb];
        ALTER ROLE [db_datareader] ADD MEMBER [LogixServiceUser];
        ALTER ROLE [db_datawriter] ADD MEMBER [LogixServiceUser];
        ALTER ROLE [db_owner] ADD MEMBER [LogixServiceUser];
    END
END
