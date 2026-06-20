-- Handle Service Account Login and User
-- We only attempt this if running on Windows.
IF (SELECT @@VERSION) LIKE '%Windows%'
    BEGIN
        BEGIN TRY
            -- Create Login for the Windows Virtual Account
            IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'NT SERVICE\LogixDb')
                BEGIN
                    DECLARE @sql NVARCHAR(MAX) = N'CREATE LOGIN [NT SERVICE\LogixDb] FROM WINDOWS WITH DEFAULT_DATABASE = ' + QUOTENAME(DB_NAME());
                    EXEC sp_executesql @sql;
                END

            -- Create the Database User for the Service
            IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'LogixServiceUser')
                BEGIN
                    CREATE USER [LogixServiceUser] FOR LOGIN [NT SERVICE\LogixDb];
                END
        END TRY
        BEGIN CATCH
            PRINT 'Note: NT SERVICE\LogixDb could not be provisioned. Skipping Windows-specific security setup.';
        END CATCH
    END
GO

-- Permissions and Schema Authorization
-- We only apply these if the user was successfully created above.
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'LogixServiceUser')
    BEGIN
        -- Grant Ingestion Permissions to the Service User
        ALTER AUTHORIZATION ON SCHEMA::logix TO [LogixServiceUser];
        ALTER AUTHORIZATION ON SCHEMA::qa TO [LogixServiceUser];

        -- Special case: The service user needs to run the migration itself
        ALTER ROLE [db_owner] ADD MEMBER [LogixServiceUser];
    END
GO

-- Secure by Default: Restrict public (all other users) to Read & Execute
-- This applies regardless of the service user's existence.
GRANT SELECT, EXECUTE ON SCHEMA::logix TO [public];
GRANT SELECT, EXECUTE ON SCHEMA::qa TO [public];
GO