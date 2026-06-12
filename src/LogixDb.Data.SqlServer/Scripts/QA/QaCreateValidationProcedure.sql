CREATE PROCEDURE [qa].[create_validation]
    @validation_class SYSNAME,
    @validation_name  SYSNAME
AS
BEGIN
    SET NOCOUNT ON;

    IF @validation_class IS NULL OR @validation_class = N''
        THROW 50000, 'validation_class is required', 1;

    IF LOWER(@validation_class) = N'qa' OR LOWER(@validation_class) = N'dbo'
        THROW 50000, 'validation_class is a reserved class name for this framework', 1;

    IF PATINDEX(N'%[^a-zA-Z0-9_]%', @validation_class) > 0
        THROW 50000, 'Invalid validation class name', 1;

    IF PATINDEX(N'%[^a-zA-Z0-9_]%', @validation_name) > 0
        THROW 50000, 'Invalid validation procedure name', 1;

    IF SCHEMA_ID(@validation_class) IS NULL
        BEGIN
            DECLARE @create_schema_sql NVARCHAR(MAX) =
                N'CREATE SCHEMA ' + QUOTENAME(@validation_class) + N';';

            EXEC sys.sp_executesql @create_schema_sql;
        END

    DECLARE @qualified_name NVARCHAR(300) =
        QUOTENAME(@validation_class) + N'.' + QUOTENAME(@validation_name);

    IF OBJECT_ID(@qualified_name, N'P') IS NOT NULL
        THROW 50000, 'Validation procedure %s already exists', 1;

    DECLARE @sql NVARCHAR(MAX) =
        N'CREATE PROCEDURE ' + @qualified_name + N'
    @vars qa.variables READONLY
AS
BEGIN
	-- Reads a variable called ''version_id'' from the provided variables table and handles errors (key not found, invalid cast)
	DECLARE @version_id INT;
	EXEC qa.get_variable_as_int @vars, ''version_id'', @version_id OUT

    /*
    Recommended pattern:
    - Query data set to validate.
	- Optionally insert data into a temp table (e.g. #validation_data) which makes multiple validations easier.
    - Evaluate resulting data set count and rows as needed.
    - For each failure case, emit a relevant failure message associated result data as JSON payload
	- By default, emitting no failure results in success when executed by the runner.
	- If needed, you can emit a custom success message with associated result data as JSON payload.
    */

	-- Replace with your validation query
    SELECT
        CONVERT(nvarchar(128), N''example_key'') AS item_key,
        CONVERT(nvarchar(max), N''actual_value'') AS actual_value,
        CONVERT(nvarchar(max), N''expected_value'') AS expected_value
    INTO #validation_data

	-- Replace with your validation logic
    IF EXISTS (SELECT 1 FROM #validation_data WHERE 1 = 0)
    BEGIN
		-- ''qa.emit_failure'' is a built in function that simplifies the contract for returning valid qa.outcome UDT
        SELECT *
        FROM qa.emit_failure(
            ''Validation failed message that should explain why validation failed.'',
			/*
			This following statement converts the failed result set into a JSON payload that can be inspected later.
			Note that we are only returning the rows that violate the expectation or test case. 
			This is best practice because it directs user to the violating records.
			*/
            (SELECT * FROM #validation_data  WHERE 1 = 0 FOR JSON PATH) 
        );
    END

    DROP TABLE #validation_data;
END;';

    EXEC sys.sp_executesql @sql;
END;
GO
