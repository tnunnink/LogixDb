-- noinspection SqlWithoutWhereForFile

CREATE OR ALTER PROCEDURE [qa].[run_validations]
    @vars qa.variables READONLY,
    @vals qa.validations READONLY,
    @run_name SYSNAME
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM @vals)
        THROW 50000, 'No validations provided', 1;

    DECLARE @run_id BIGINT;
    DECLARE @variables_data NVARCHAR(MAX);
    DECLARE @variables_hash VARBINARY(32);
    DECLARE @validation_name SYSNAME;
    DECLARE @results qa.results;
    DECLARE @has_error BIT = 0;

    -- Hash the variable data so that we can compare which runs had same environment
    SELECT @variables_data = (SELECT variable_name, variable_value FROM @vars ORDER BY variable_name FOR JSON PATH);
    SET @variables_data = ISNULL(@variables_data, N'[]');
    SET @variables_hash = HASHBYTES(N'SHA2_256', @variables_data);

    -- Post to the run table to indicate that the run has started
    INSERT INTO [qa].validation_run(run_name, run_status, variables_data, variables_hash)
    VALUES (@run_name, N'Running', @variables_data, @variables_hash);
    SET @run_id = SCOPE_IDENTITY();

    DECLARE validation_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT validation_name FROM @vals;

    OPEN validation_cursor;
    FETCH NEXT FROM validation_cursor
        INTO @validation_name;

    -- Run each validation, skip errors but flag so that we can mark run as errored in the end.
    WHILE @@FETCH_STATUS = 0
    BEGIN
        DELETE FROM @results;

        BEGIN TRY
            EXEC [qa].[execute_validation] @vars = @vars, @validation_name = @validation_name, @run_id = @run_id
        END TRY
        BEGIN CATCH
            SET @has_error = 1;
            FETCH NEXT FROM validation_cursor INTO @validation_name;
            CONTINUE;
        END CATCH

        FETCH NEXT FROM validation_cursor INTO @validation_name;
    END

    CLOSE validation_cursor;
    DEALLOCATE validation_cursor;

    UPDATE [qa].validation_run
    SET run_status =
            CASE
            WHEN @has_error = 1 THEN N'Error'
            WHEN EXISTS (SELECT 1 FROM [qa].validation_result WHERE run_id = @run_id AND is_success = 0) THEN N'Failed'
            ELSE N'Passed' END,
        completed_on = SYSUTCDATETIME()
    WHERE run_id = @run_id;

    RETURN @run_id
END;
GO