CREATE PROCEDURE [qa].[get_override_variable]
    @vars qa.variables READONLY,
    @id INT,
    @variable NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @variable = (
        SELECT variable_value
        FROM @vars
        WHERE variable_name = (SELECT CONCAT(OBJECT_SCHEMA_NAME(@id), '.', OBJECT_NAME(@id)))
    )
END;
GO
