DROP FUNCTION [qa].[inspect_result]

CREATE PROCEDURE [qa].[inspect_result]
    @result_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @json NVARCHAR(MAX);

    SELECT @json = result_details
    FROM [qa].[validation_result]
    WHERE result_id = @result_id;

    IF @json IS NULL
        THROW 50001, 'The requested validation result was not found or has no result details.', 1;

    IF ISJSON(@json) <> 1
        THROW 50002, 'The validation result does not contain valid JSON.', 1;

    IF EXISTS (SELECT 1 FROM OPENJSON(@json) WHERE [type] <> 5)
        THROW 50003, 'The validation result must be a JSON array of objects.', 1;

    CREATE TABLE #result_values
    (
        property_order INT IDENTITY (1, 1) NOT NULL,
        row_index      INT                 NOT NULL,
        property_name  NVARCHAR(4000)      NOT NULL,
        property_value NVARCHAR(MAX)       NULL,
        property_type  INT                 NOT NULL
    );

    INSERT INTO #result_values
    (
        row_index,
        property_name,
        property_value,
        property_type
    )
    SELECT
        TRY_CONVERT(INT, result_row.[key]),
        result_property.[key],
        result_property.[value],
        result_property.[type]
    FROM OPENJSON(@json) AS result_row
         CROSS APPLY OPENJSON(result_row.[value]) AS result_property
    WHERE result_row.[type] = 5;

    IF EXISTS (SELECT 1 FROM #result_values WHERE LEN(property_name) > 128)
        THROW 50004, 'A JSON property name exceeds the SQL Server identifier limit of 128 characters.', 1;

    IF NOT EXISTS (SELECT 1 FROM #result_values)
        THROW 50005, 'The validation result contains no properties to display.', 1;

    DECLARE @columns NVARCHAR(MAX);

    SELECT
        @columns = STRING_AGG(CONVERT(NVARCHAR(MAX), QUOTENAME(property_name)), N', ')
                              WITHIN GROUP (ORDER BY first_property_order)
    FROM (SELECT
              property_name,
              MIN(property_order) AS first_property_order
          FROM #result_values
          GROUP BY property_name) AS properties;

    DECLARE @sql NVARCHAR(MAX) = N'
        SELECT ' + @columns + N'
        FROM
        (
            SELECT
                row_index,
                property_name,
                property_value
            FROM #result_values
        ) AS source
        PIVOT
        (
            MAX(property_value)
            FOR property_name IN (' + @columns + N')
        ) AS pivoted
        ORDER BY row_index;';

    EXEC sys.sp_executesql @sql;
END;
GO