CREATE FUNCTION [qa].[emit_failure](
    @result_message nvarchar(MAX),
    @result_details nvarchar(MAX)
)
RETURNS TABLE
AS RETURN
(
    SELECT CONVERT(bit, 0)		    AS is_success,
    ISNULL(@result_message, N'')  AS result_message,
    @result_details               AS result_details
)
GO
