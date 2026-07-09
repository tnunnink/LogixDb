CREATE FUNCTION [logix].[default_value]
(
    @DataType NVARCHAR(128)
)
    RETURNS NVARCHAR(256)
AS
BEGIN
    RETURN CASE
           WHEN @DataType IN ('BOOL', 'SINT', 'INT', 'DINT', 'LINT') THEN '0'
           WHEN @DataType IN ('USINT', 'UINT', 'UDINT', 'ULINT') THEN '0'
           WHEN @DataType IN ('DT', 'LDT', 'TIME', 'TIME32', 'LTIME') THEN '0'
           WHEN @DataType IN ('REAL', 'LREAL') THEN '0.0'
        END;
END;