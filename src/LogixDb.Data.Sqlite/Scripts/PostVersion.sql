INSERT INTO target_version
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
        (SELECT target_id FROM target WHERE target_key = @TargetKey),
        (SELECT IFNULL(MAX(version_number), 0) + 1
         FROM target_version
         WHERE target_id = (SELECT target_id FROM target WHERE target_key = @TargetKey)
         ),
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
    )
RETURNING version_id;