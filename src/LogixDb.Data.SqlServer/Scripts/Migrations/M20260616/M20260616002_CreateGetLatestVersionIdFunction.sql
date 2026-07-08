CREATE OR ALTER FUNCTION [logix].[get_latest_version_id] (@target_name NVARCHAR(128))
RETURNS INT
AS
BEGIN
    DECLARE @version_id INT;

    SELECT TOP 1 @version_id = version_id
    FROM [logix].[target_version]
    WHERE [target_name] = @target_name
    ORDER BY [version_number] DESC;

    RETURN @version_id;
END;
