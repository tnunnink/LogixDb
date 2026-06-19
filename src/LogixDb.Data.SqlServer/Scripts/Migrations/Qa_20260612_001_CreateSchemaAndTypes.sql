IF SCHEMA_ID('qa') IS NULL EXEC ('CREATE SCHEMA [qa]')
GO

CREATE TYPE [qa].[outcome] AS TABLE
(
    is_success     BIT           NOT NULL,
    result_message NVARCHAR(MAX) NOT NULL,
    result_details NVARCHAR(MAX)
)
GO

CREATE TYPE [qa].[validations] AS TABLE
(
    validation_name SYSNAME NOT NULL
)
GO

CREATE TYPE [qa].[variables] AS TABLE
(
    variable_name  SYSNAME NOT NULL PRIMARY KEY,
    variable_value NVARCHAR(MAX)
)
GO
