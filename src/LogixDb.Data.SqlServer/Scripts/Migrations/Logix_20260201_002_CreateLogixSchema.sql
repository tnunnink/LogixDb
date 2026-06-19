IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'logix')
    BEGIN
        EXEC('CREATE SCHEMA [logix]');
    END
GO