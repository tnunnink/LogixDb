CREATE TABLE [logix].[module_connection]
(
    [connection_id]                      BIGINT IDENTITY (1, 1) NOT NULL,
    [module_id]                          BIGINT                 NOT NULL,
    [connection_name]                    NVARCHAR(256)          NULL,
    [rpi]                                INT                    NULL,
    [connection_type]                    NVARCHAR(64)           NULL,
    [connection_priority]                NVARCHAR(64)           NULL,
    [transmission_type]                  NVARCHAR(64)           NULL,
    [production_trigger]                 NVARCHAR(64)           NULL,
    [output_redundant_owner]             BIT                    NULL,
    [unicast]                            BIT                    NULL,
    [programatically_send_event_trigger] BIT                    NULL,
    [event_id]                           INT                    NULL,
    [input_tag]                          NVARCHAR(256)          NULL,
    [input_size]                         INT                    NULL,
    [input_suffix]                       NVARCHAR(64)           NULL,
    [output_tag]                         NVARCHAR(256)          NULL,
    [output_size]                        INT                    NULL,
    [output_suffix]                      NVARCHAR(64)           NULL,
    [connection_path]                    NVARCHAR(512)          NULL,
    [record_hash]                        NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_module_connection] PRIMARY KEY CLUSTERED ([connection_id] ASC),
    CONSTRAINT [FK_module_connection_module] FOREIGN KEY ([module_id]) REFERENCES [logix].[module] ([module_id]) ON DELETE CASCADE
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_module_connection_module_id_record_hash]
    ON [logix].[module_connection] ([module_id] ASC, [record_hash] ASC);
