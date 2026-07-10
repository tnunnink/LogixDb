CREATE PROCEDURE [qa].[create_suite]
    @suite_name SYSNAME
AS
BEGIN
    SET NOCOUNT ON;

    IF @suite_name IS NULL OR @suite_name = N''
        THROW 50000, '@suite_name is required', 1;

    IF PATINDEX(N'%[^a-zA-Z0-9_]%', @suite_name) > 0
        THROW 50000, 'Invalid suite name', 1;

    -- Initialize the common 'suite' schema that will contain all suites
    IF SCHEMA_ID('suite') IS NULL
    BEGIN
        EXEC sys.sp_executesql N'CREATE SCHEMA [suite];';
    END
        
    DECLARE @qualified_name NVARCHAR(300) = N'[suite].' + QUOTENAME(@suite_name);

    IF OBJECT_ID(@qualified_name, N'P') IS NOT NULL
        THROW 50000, 'Suite procedure %s already exists', 1;

    DECLARE @sql NVARCHAR(MAX) = N'CREATE PROCEDURE ' + @qualified_name + N'
AS
BEGIN
	SET NOCOUNT ON;

	----------------------------------------------------------------------------------
	-- Step 1: Configure Variables 
	----------------------------------------------------------------------------------
	-- Configure variables to drive how validation are executed.
	-- Each variable can be tuned per suite to get data statically 
	-- or dynamically using nested queries or functions.
	----------------------------------------------------------------------------------
    DECLARE @vars qa.variables;
	INSERT INTO @vars VALUES 
	-- Most validations probably require a ''version_id'' to get content to verify. 
	-- Each run can get latest version to check. This data is pinned in validation run table so they can be rerun.
	-- Replace with project target key from target table as needed.
	(''version_id'', (SELECT CAST(logix.latest_version_id(''MyTarget'') AS NVARCHAR(max)))), 

  -- Get the project version as reference to compare against.
	(''template_id'', (SELECT CAST(logix.latest_version_id(''MyTemplate'') AS NVARCHAR(max)))),

	-- Configure static variable values directly.
	(''static_variable'', ''SomeValue''),

	-- Configure dynamic JSON serialized variable that validation know how to parse.
	(''json_variable'', (SELECT col_name FROM table_name FOR JSON PATH));


	----------------------------------------------------------------------------------
	-- Step 2: Setup Validations
	----------------------------------------------------------------------------------
	-- Configure which validation to run for this suite. 
	-- You can select all validations using the list validations view.
	-- The nice part about this approach is that newly added validations get run
	-- without having to change this procedure.
	----------------------------------------------------------------------------------
	DECLARE @vals qa.validations;
	INSERT INTO @vals
	SELECT qualified_name 
	FROM qa.list_validations

	-- Example of how to explicitly exclude certain validations from this suite.
	--DELETE FROM @vals 
	--WHERE validation_name IN (
	--	''[validate].[module_configs]'',
	--	''[validate].[tag_consistency]''
	--)

	-- Example of how you could use SQL to delete validations more dynamically
	--DELETE FROM @vals 
	--WHERE validation_name LIKE ''%tag%''


	----------------------------------------------------------------------------------
	-- Step 3: Run Suite
	----------------------------------------------------------------------------------
	-- This part does not have to change. It just executes all the 
	-- provided validations using the configured variables, which will then
	-- post results to the validation_run and validation_result tables.
	--
	-- However, it is completely possible to produce multiple "runs" by
	-- calling this procedure multiple times within this "suite" procedure.
	-- Each run can be executed with different sets of validations and 
	-- variables as needed.
	----------------------------------------------------------------------------------
  -- Makes the run name the name of the suite for consistency
	DECLARE @run SYSNAME = OBJECT_NAME(@@PROCID); 
	EXEC qa.run_validations @vars, @vals, @run
END;';

    EXEC sys.sp_executesql @sql;
END;
GO
