CREATE OR ALTER FUNCTION [logix].[latest_version_id]
(
    @TargetKey NVARCHAR(256)
)
    RETURNS INT
AS
BEGIN
    DECLARE @VersionId INT;

    SELECT TOP 1 @VersionId = version_id
    FROM logix.target t
    JOIN logix.target_version tv on tv.target_id = t.target_id
    WHERE t.target_key = @TargetKey
    ORDER BY [version_number] DESC;

    RETURN @VersionId;
END;