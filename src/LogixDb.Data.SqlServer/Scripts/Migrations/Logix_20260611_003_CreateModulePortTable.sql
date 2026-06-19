CREATE TABLE [logix].[module_port]
(
    [port_id]     BIGINT IDENTITY (1, 1) NOT NULL,
    [module_id]   BIGINT                 NOT NULL,
    [port_number] SMALLINT               NOT NULL,
    [port_type]   NVARCHAR(64)           NULL,
    [address]     NVARCHAR(256)          NULL,
    [upstream]    BIT                    NULL,
    [bus_size]    TINYINT                NULL,
    [record_hash] NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_module_port] PRIMARY KEY CLUSTERED ([port_id] ASC),
    CONSTRAINT [FK_module_port_module] FOREIGN KEY ([module_id]) REFERENCES [logix].[module] ([module_id]) ON DELETE CASCADE
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_module_port_module_id_record_hash]
    ON [logix].[module_port] ([module_id] ASC, [record_hash] ASC);
