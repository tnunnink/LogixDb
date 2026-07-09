CREATE FUNCTION [logix].[is_atomic]
(
    @DataType NVARCHAR(128)
)
    RETURNS BIT
AS
BEGIN
    RETURN IIF(@DataType IN
               ('BOOL','SINT','INT','DINT','LINT','REAL',
                'LREAL','USINT','UINT','UDINT','ULINT',
                'DT','LDT','TIME','TIME32','LTIME'), 1, 0);
END;