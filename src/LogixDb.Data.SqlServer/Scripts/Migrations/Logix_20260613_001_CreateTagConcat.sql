CREATE OR ALTER FUNCTION [logix].[tag_concat]
(
    @TagName NVARCHAR(256),
    @MemberPath NVARCHAR(256)
)
RETURNS NVARCHAR(256)
AS
BEGIN
    DECLARE @Separator NVARCHAR(1) = CASE
        WHEN @MemberPath IS NULL OR @MemberPath = '' THEN ''
        WHEN @TagName IS NULL OR @TagName = '' THEN ''
        WHEN CHARINDEX('[', @MemberPath) = 1 THEN ''
        ELSE '.'
    END;
    
    DECLARE @TagPath NVARCHAR(256) = CONCAT(@TagName, @Separator, @MemberPath)
    RETURN @TagPath
END;
