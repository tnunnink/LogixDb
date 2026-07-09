CREATE FUNCTION [logix].[tag_base]
(
    @TagName NVARCHAR(256)
)
RETURNS NVARCHAR(256)
AS
BEGIN
    -- Gets the first index of the member separator ('.' or '[') which we can use to extract the base tag name.
    DECLARE @Separator INT = (SELECT MIN(NULLIF(CHARINDEX(c, @TagName), 0)) FROM (VALUES ('.'), ('[')) AS chars(c))

    -- Compute the length of the base tag. If no member separator exists, return the full tag length.
    DECLARE @Length INT = IIF(@Separator IS NOT NULL, @Separator - 1, LEN(@TagName));

    -- Return the left side of the tag name to the first separator or the end of the string.
    RETURN (SELECT LEFT(@TagName, @Length));
END;