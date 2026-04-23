SELECT t.target_id              [TargetId],
       v.version_id             [VersionId],
       COALESCE(i.instance_id, 0) [InstanceId],
       t.target_key             [TargetKey],
       v.version_number         [VersionNumber],
       v.target_type            [TargetType],
       v.target_name            [TargetName],
       v.is_partial             [IsPartial],
       v.schema_revision        [SchemaRevision],
       v.software_revision      [SoftwareRevision],
       v.export_date            [ExportDate],
       v.export_options         [ExportOptions],
       v.import_date            [ImportDate],
       v.import_user            [ImportUser],
       v.import_machine         [ImportMachine],
       v.source_hash            [SourceHash],
       v.source_data            [SourceData]
FROM target t
         JOIN target_version v on t.target_id = v.target_id
         LEFT JOIN target_instance i on v.version_id = i.version_id
WHERE t.target_key = @TargetKey
  AND v.version_number = @VersionNumber