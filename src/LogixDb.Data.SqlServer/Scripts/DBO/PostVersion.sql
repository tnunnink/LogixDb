DECLARE @TargetId INT = (SELECT target_id FROM dbo.target WHERE target_key = @TargetKey);
DECLARE @VersionNumber INT = (SELECT COALESCE(MAX(version_number), 0) + 1 FROM dbo.target_version WHERE target_id = @TargetId);

INSERT INTO dbo.target_version
(
    target_id,
    version_number,
    target_type,
    target_name,
    is_partial,
    schema_revision,
    software_revision,
    export_date,
    export_options,
    import_date,
    import_user,
    import_machine,
    source_hash,
    source_data
)
VALUES
    (
        @TargetId,
        @VersionNumber,
        @TargetType,
        @TargetName,
        @IsPartial,
        @SchemaRevision,
        @SoftwareRevision,
        @ExportDate,
        @ExportOptions,
        @ImportDate,
        @ImportUser,
        @ImportMachine,
        @SourceHash,
        @SourceData
    );

SELECT CAST(SCOPE_IDENTITY() AS INT) AS VersionId, @VersionNumber AS VersionNumber;