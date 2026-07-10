CREATE PROCEDURE [qa].[duplicate_suite]
    @suite_name SYSNAME,
    @new_name SYSNAME
AS
BEGIN
    SET NOCOUNT ON;

    IF @new_name IS NULL OR @new_name = N''
        THROW 50000, '@new_name is required', 1;

    IF PATINDEX(N'%[^a-zA-Z0-9_]%', @new_name) > 0
        THROW 50000, 'Invalid suite name', 1;

    DECLARE @current NVARCHAR(300) = N'[suite].' + QUOTENAME(@suite_name);
    DECLARE @new NVARCHAR(300) = N'[suite].' + QUOTENAME(@new_name);
    
    IF OBJECT_ID(@current, N'P') IS NULL
        THROW 50000, 'Suite procedure does not exist', 1;

    IF OBJECT_ID(@new, N'P') IS NOT NULL
        THROW 50000, 'Suite procedure already exists', 1;

    DECLARE @sql NVARCHAR(MAX);

    -- Get SQL DDL for current procedure
    SELECT @sql = OBJECT_DEFINITION(OBJECT_ID(@current));

    -- Replace the old name with the new name
    SET @sql = REPLACE(@sql, @current, @new);

    -- If the first line is ALTER, change it to CREATE
    IF CHARINDEX('ALTER', @sql) > 0
    BEGIN
        SET @sql = REPLACE(@sql, 'ALTER PROCEDURE', 'CREATE PROCEDURE');
    END

    -- Execute the new procedure
    EXEC sp_executesql @sql;
END;
GO
