CREATE PROCEDURE [qa].[generate_approval]
(
    @ResultId BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM qa.validation_result WHERE result_id = @ResultId)
        THROW 50000, 'Validation result not found', 1;

    IF (SELECT is_success FROM qa.validation_result WHERE result_id = @ResultId) = 1
        THROW 50000, 'Only failed validations can be used to generate an approval', 1;

    SELECT
        FORMATMESSAGE('(''%s'', ''%s'')', validation_name, (SELECT qa.hash(COALESCE(result_details, N'[]'))))
        AS [Approval]
    FROM qa.validation_result
    WHERE result_id = @ResultId
END;