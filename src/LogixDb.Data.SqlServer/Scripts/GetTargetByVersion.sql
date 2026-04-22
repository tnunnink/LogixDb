SELECT 
    s.version_id [VersionId],
    ti.instance_id [InstanceId],
    t.target_id [TargetId],
    s.version_number [VersionNumber],
    t.target_key [TargetKey],
    target_type [TargetType],
    target_name [TargetName],
    is_partial [IsPartial],
    schema_revision [SchemaRevision],
    software_revision [SoftwareRevision],
    export_date [ExportDate],
    export_options [ExportOptions],
    import_date [ImportDate],
    import_user [ImportUser],
    import_machine [ImportMachine],
    source_hash [SourceHash], 
    source_data [SourceData] 
FROM target_version s
JOIN target t on t.target_id = s.target_id
LEFT JOIN target_instance ti ON ti.version_id = s.version_id
WHERE t.target_key = @TargetKey and s.version_number = @VersionNumber
