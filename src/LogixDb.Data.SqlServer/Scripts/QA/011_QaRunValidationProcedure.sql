CREATE PROCEDURE [qa].[run_validation]
    @vars     qa.variables READONLY,
    @validation_name sysname,
    @run_id bigint = NULL,
    @run_name sysname = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @validation_name IS NULL
        THROW 50000, 'No validation provided.', 1;

    DECLARE @variables_data nvarchar(max);
    DECLARE @variables_hash varbinary(32);
    DECLARE @sql nvarchar(max);
    DECLARE @outcome qa.outcome;
    DECLARE @has_error bit = 0;

    IF @run_id IS NULL
    BEGIN
        SELECT @variables_data =
               (
                   SELECT variable_name, variable_value
                   FROM @vars
                   ORDER BY variable_name
                   FOR JSON PATH
               );

        SET @variables_data = ISNULL(@variables_data, N'[]');
        SET @variables_hash = HASHBYTES(N'SHA2_256', @variables_data);

        INSERT INTO [qa].validation_run (run_name, run_status, variables_data, variables_hash)
        VALUES (@run_name, N'Running', @variables_data, @variables_hash);
        SET @run_id = SCOPE_IDENTITY();
    END

    IF OBJECT_ID(@validation_name, N'P') IS NULL
        BEGIN
            SET @has_error = 1;
            INSERT INTO [qa].validation_result (run_id, validation_name, is_success, result_message, result_details)
            VALUES (@run_id, @validation_name, 0, N'Validation procedure does not exist.', N'[]');
        END

    IF @has_error = 0
        BEGIN
            BEGIN TRY
                SET @sql = N'EXEC ' + @validation_name + N' @vars = @vars';
                INSERT INTO @outcome
                    EXEC sys.sp_executesql @sql, N'@vars qa.variables READONLY', @vars = @vars;
            END TRY
            BEGIN CATCH
                SET @has_error = 1;
                INSERT INTO [qa].validation_result (run_id, validation_name, is_success, result_message, result_details)
                VALUES (@run_id, @validation_name, 0, ERROR_MESSAGE(), N'[]');
            END CATCH
        END

    IF @has_error = 0 AND (SELECT COUNT(*) FROM @outcome) = 0
        BEGIN
            INSERT INTO [qa].validation_result (run_id, validation_name, is_success, result_message, result_details)
            VALUES(@run_id, @validation_name, 1, N'Validation completed successfully with no results.', N'[]');
        END

    IF @has_error = 0
        BEGIN
            INSERT INTO [qa].validation_result (run_id, validation_name, is_success, result_message, result_details)
            SELECT @run_id, @validation_name, is_success, result_message, result_details
            FROM @outcome;
        END

    UPDATE [qa].validation_run
    SET
        run_status =
            CASE
                WHEN @has_error = 1 THEN N'Error'
                WHEN EXISTS (
                    SELECT 1
                    FROM [qa].validation_result
                    WHERE run_id = @run_id
                      AND is_success = 0
                ) THEN N'Failed'
                ELSE N'Passed'
                END,
        completed_on = SYSUTCDATETIME()
    WHERE run_id = @run_id;

    SELECT
        result_id,
        run_id,
        validation_name,
        execution_time,
        is_success,
        result_message,
        result_details
    FROM [qa].validation_result
    WHERE run_id = @run_id
    ORDER BY result_id;
END;
GO
