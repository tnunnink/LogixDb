CREATE VIEW [qa].[list_validations] AS
SELECT
    SCHEMA_NAME(p.schema_id)					AS validation_class,
    p.name										AS validation_name,
    SCHEMA_NAME(p.schema_id) + N'.' + p.name	AS qualified_name
FROM sys.procedures AS p
     JOIN sys.parameters AS param ON param.object_id = p.object_id
WHERE SCHEMA_NAME(p.schema_id) <> N'qa'
  AND SCHEMA_NAME(p.schema_id) <> N'dbo'
  AND param.name = N'@vars'
  AND param.parameter_id = 1
  AND param.is_readonly = 1
  AND TYPE_NAME(param.user_type_id) = N'variables'
  AND (SELECT COUNT(*) FROM sys.parameters AS pcount WHERE pcount.object_id = p.object_id) = 1
GO
