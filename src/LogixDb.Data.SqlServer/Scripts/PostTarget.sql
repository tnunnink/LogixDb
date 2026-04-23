IF NOT EXISTS (SELECT 1
               FROM target
               WHERE target_key = @TargetKey)
    BEGIN
        INSERT INTO target (target_id, target_key) VALUES (@TargetId, @TargetKey)
    END

INSERT INTO target_version (version_id,
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
                            source_data)
VALUES (@VersionId,
        (SELECT target_id FROM target WHERE target_key = @TargetKey),
        (SELECT COALESCE(MAX(version_number), 0) + 1
         FROM target_version
         WHERE target_id = (SELECT target_id FROM target WHERE target_key = @TargetKey)),
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
        @SourceData);
