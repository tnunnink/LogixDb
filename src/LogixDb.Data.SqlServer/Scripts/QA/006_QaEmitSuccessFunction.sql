CREATE FUNCTION [qa].[emit_success](
    @result_message nvarchar(max)
)
    RETURNS TABLE
        AS
        RETURN
        (
        SELECT CONVERT(bit, 1) AS is_success,
               ISNULL(@result_message, N'') AS result_message,
               CONVERT(nvarchar(max), '[]') AS result_details
        )
GO
