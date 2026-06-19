CREATE PROCEDURE [qa].[get_variable_as_int]
    @vars qa.variables READONLY,
    @key NVARCHAR(MAX),
    @variable INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @value NVARCHAR(MAX) = (
        SELECT TOP 1 variable_value
        FROM @vars
        WHERE variable_name = @key
    )

    DECLARE @message AS NVARCHAR(4000);

    IF @value IS NULL
        BEGIN
            SET @message = FORMATMESSAGE('''%s'' was not found in provided variables', @key);
            THROW 50000, @message, 1;
        END

    SELECT @variable = TRY_CAST(@value AS INT);

    IF @variable IS NULL
        BEGIN
            SET @message = FORMATMESSAGE('''%s'' could not be parsed as integer for key ''%s''' ,@value, @key);
            THROW 50000, @message, 1;
        END
END;
GO
