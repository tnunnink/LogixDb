CREATE TABLE [qa].[validation_run] (
    run_id           BIGINT IDENTITY PRIMARY KEY,
    run_name         SYSNAME NOT NULL,
    run_status       SYSNAME NOT NULL,
    executed_by      SYSNAME NOT NULL DEFAULT SUSER_SNAME(),
    executed_on      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    completed_on     DATETIME2 NULL,
    environment_data NVARCHAR(MAX) NOT NULL,
    environment_hash VARBINARY(32) NOT NULL,
    CONSTRAINT [ck_qa_validation_run_run_status] CHECK ([run_status] IN ('Error', 'Partial', 'Failed', 'Passed', 'Running'))
)
GO

CREATE INDEX [ix_qa_validation_run_environment_hash] ON [qa].[validation_run] ([environment_hash])
GO
