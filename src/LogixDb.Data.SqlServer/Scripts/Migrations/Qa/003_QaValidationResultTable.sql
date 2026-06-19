CREATE TABLE [qa].[validation_result] (
    result_id       BIGINT IDENTITY PRIMARY KEY,
    run_id          BIGINT NOT NULL,
    validation_name SYSNAME NOT NULL,
    execution_time  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    is_success      BIT NOT NULL,
    result_message  NVARCHAR(MAX) NOT NULL,
    result_details  NVARCHAR(MAX) NULL,
    CONSTRAINT [fk_qa_validation_result_run] FOREIGN KEY (run_id) REFERENCES [qa].[validation_run] (run_id)
)
GO

CREATE INDEX [ix_qa_validation_result_run_validation] ON [qa].[validation_result] ([run_id], [validation_name])
GO

CREATE INDEX [ix_qa_validation_result_validation_name] ON [qa].[validation_result] ([validation_name])
GO
