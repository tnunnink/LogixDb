CREATE FUNCTION [logix].[tag_member]
(
    @TagName NVARCHAR(256)
)
RETURNS NVARCHAR(256)
AS
BEGIN
    -- Gets the first index of the member separator ('.' or '[')
    DECLARE @Separator INT = (SELECT MIN(NULLIF(CHARINDEX(c, @TagName), 0)) FROM (VALUES ('.'), ('[')) AS chars(c))

    -- If no separator exists, return an empty string. Otherwise, return everything after the separator.
    RETURN IIF(@Separator IS NULL, '', SUBSTRING(@TagName, @Separator + 1, LEN(@TagName)));
END;