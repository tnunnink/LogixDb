CREATE PROCEDURE [qa].[run_validations]
    @vars     qa.variables READONLY,
    @vals     qa.validations READONLY,
    @run_name sysname = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM @vals)
        THROW 50000, 'No validations provided.', 1;

    DECLARE @run_id bigint;
    DECLARE @environment_data nvarchar(max);
    DECLARE @environment_hash varbinary(32);
    DECLARE @validation_name nvarchar(300);
    DECLARE @sql nvarchar(max);
    DECLARE @outcome qa.outcome;
    DECLARE @has_error bit = 0;

    SELECT
        @environment_data =
        (
            SELECT
                variable_name,
                variable_value
            FROM @vars
            ORDER BY variable_name
            FOR JSON PATH
        );

    SET @environment_data = ISNULL(@environment_data, N'[]');
    SET @environment_hash = HASHBYTES(N'SHA2_256', @environment_data);

    INSERT INTO [qa].validation_run
    (
        run_name,
        run_status,
        environment_data,
        environment_hash
    )
    VALUES
        (
            @run_name,
            N'Running',
            @environment_data,
            @environment_hash
        );

    SET @run_id = SCOPE_IDENTITY();

    DECLARE validation_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT validation_name
        FROM @vals;

    OPEN validation_cursor;
    FETCH NEXT FROM validation_cursor INTO @validation_name;

    WHILE @@FETCH_STATUS = 0
        BEGIN
            DELETE FROM @outcome;

            IF OBJECT_ID(@validation_name, N'P') IS NULL
                BEGIN
                    SET @has_error = 1;

                    INSERT INTO [qa].validation_result (run_id, validation_name, is_success, result_message, result_details)
                    VALUES (@run_id, @validation_name, 0, N'Validation procedure does not exist.', N'[]');

                    FETCH NEXT FROM validation_cursor INTO @validation_name;
                    CONTINUE;
                END

            BEGIN TRY

                SET @sql = N'EXEC [qa].[run_validation] @vars = @vars, @validation_name = @validation_name, @run_id = @run_id';

                EXEC sys.sp_executesql
                         @sql,
                         N'@vars qa.variables READONLY, @validation_name sysname, @run_id bigint',
                         @vars = @vars,
                         @validation_name = @validation_name,
                         @run_id = @run_id;

            END TRY
            BEGIN CATCH
                SET @has_error = 1;

                INSERT INTO [qa].validation_result (run_id, validation_name, is_success, result_message, result_details)
                VALUES (@run_id, @validation_name, 0, ERROR_MESSAGE(), N'[]');

                FETCH NEXT FROM validation_cursor INTO @validation_name;
                CONTINUE;
            END CATCH

            FETCH NEXT FROM validation_cursor INTO @validation_name;
        END

    CLOSE validation_cursor;
    DEALLOCATE validation_cursor;

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
