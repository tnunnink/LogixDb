CREATE PROCEDURE [qa].[get_variable]
    @vars qa.variables READONLY,
    @key NVARCHAR(MAX),
    @variable NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @variable = (
        SELECT TOP 1 variable_value
        FROM @vars
        WHERE variable_name = @key
    )

    IF @variable IS NULL
        BEGIN
            DECLARE @message AS NVARCHAR(4000) = FORMATMESSAGE('''%s'' was not found in provided variables', @key);
            RAISERROR(@message, 16, 1);
        END
END;
GO
