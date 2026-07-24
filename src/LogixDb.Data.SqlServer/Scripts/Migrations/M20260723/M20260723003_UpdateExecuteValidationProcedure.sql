ALTER PROCEDURE [qa].[execute_validation]
    @vars qa.variables READONLY,
    @validation_name SYSNAME,
    @run_id BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Abort with error if no validation was provided
    IF @validation_name IS NULL OR @validation_name = N''
        THROW 50000, N'No validation provided', 1;

    -- Abort with error if the validation procedure does not exist
    IF OBJECT_ID(@validation_name, N'P') IS NULL
    BEGIN
        INSERT INTO [qa].validation_result (run_id, validation_name, is_success, result_message, result_details)
        VALUES (@run_id, @validation_name, 0, N'Validation procedure does not exist', N'[]');
        THROW 50000, N'Validation procedure does not exist', 1;
    END

    -- Try to execute the validation and post any results to the result table.
    -- Rethrow the error to bubble up to the runner so that it can mark the run as errored.
    DECLARE @results qa.results;

    BEGIN TRY
        DECLARE @sql NVARCHAR(MAX) = N'EXEC ' + @validation_name + N' @vars = @vars';
        INSERT INTO @results
            EXEC sys.sp_executesql @sql, N'@vars qa.variables READONLY', @vars = @vars;
    END TRY
    BEGIN CATCH
        INSERT INTO [qa].validation_result (run_id, validation_name, is_success, result_message, result_details)
        VALUES (@run_id, @validation_name, 0, ERROR_MESSAGE(), N'[]');
        THROW;
    END CATCH

    -- Insert results if any were returned from the validation. These could be failures or successes.
    IF (SELECT COUNT(*) FROM @results) > 0
    BEGIN
        INSERT INTO [qa].validation_result (run_id, validation_name, is_success, result_message, result_details)
        SELECT @run_id, @validation_name, is_success, result_message, result_details
        FROM @results;
        RETURN;
    END

    -- No emitted results assume success to make writing validation easier (if nothing failed, then it passed).
    INSERT INTO [qa].validation_result (run_id, validation_name, is_success, result_message, result_details)
    VALUES (@run_id, @validation_name, 1, N'Passed', N'[]');
END;
GO