CREATE PROCEDURE [qa].[rerun_validations] @run_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @vars qa.variables;
    DECLARE @vals qa.validations;
    DECLARE @run_name SYSNAME;
    DECLARE @variables_data NVARCHAR(MAX);

    -- Get run data
    SELECT @run_name = run_name,
           @variables_data = variables_data
    FROM [qa].validation_run
    WHERE run_id = @run_id;

    IF @variables_data IS NULL
        THROW 50000, 'Validation run not found.', 1;

    -- Deserialize variables
    INSERT INTO @vars (variable_name, variable_value)
    SELECT [key], [value]
    FROM OPENJSON(@variables_data)
    WITH (
        [key] SYSNAME '$.variable_name',
        [value] NVARCHAR(MAX) '$.variable_value'
    );

    -- Get validations from results
    INSERT INTO @vals (validation_name)
    SELECT DISTINCT validation_name
    FROM [qa].validation_result
    WHERE run_id = @run_id;

    IF NOT EXISTS (SELECT 1 FROM @vals)
        THROW 50000, 'No validation results found for the specified run.', 1;

    -- Execute validations
    EXEC [qa].[run_validations] @vars = @vars, @vals = @vals, @run_name = @run_name;
END;
GO
