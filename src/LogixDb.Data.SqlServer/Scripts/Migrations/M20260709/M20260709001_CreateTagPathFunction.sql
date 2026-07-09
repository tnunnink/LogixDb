CREATE FUNCTION [logix].[tag_path]
(
    @TagName NVARCHAR(256)
)
RETURNS NVARCHAR(256)
AS
BEGIN
    -- Gets the first index of the member separator ('.' or '[')
    DECLARE @Separator INT = (SELECT MIN(NULLIF(CHARINDEX(c, @TagName), 0)) FROM (VALUES ('.'), ('[')) AS chars(c))

    -- If no separator exists, return an empty string. 
    IF @Separator IS NULL 
        RETURN ''

    -- Otherwise, return everything after the separator (including '[' if array opening
    DECLARE @StartIndex INT = IIF(@Separator = '.', @Separator + 1, @Separator)
    RETURN SUBSTRING(@TagName, @StartIndex, LEN(@TagName));
END;