CREATE TABLE [logix].[target_component]
(
    [component_id]   TINYINT IDENTITY (1, 1) NOT NULL,
    [component_name] NVARCHAR(64)            NOT NULL,
    CONSTRAINT [PK_target_component] PRIMARY KEY CLUSTERED ([component_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_target_component_component_name]
    ON [logix].[target_component] ([component_name] ASC);
