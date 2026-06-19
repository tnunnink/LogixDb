CREATE FUNCTION [qa].[emit_success](
   @result_message nvarchar(max),
   @result_details nvarchar(max)
)
    RETURNS TABLE
        AS
        RETURN
        (
        SELECT CONVERT(bit, 1) AS is_success,
               ISNULL(@result_message, N'') AS result_message,
               @result_details AS result_details
        )
GO
