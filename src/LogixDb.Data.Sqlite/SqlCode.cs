namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides constant SQL statements for database operations within the LogixDb.Data.Sqlite namespace.
/// This class includes predefined SQL queries to perform CRUD operations and data retrieval related to targets, snapshots,
/// and their associated metadata in the SQLite database.
/// </summary>
internal static class SqlCode
{
    internal const string EnsureTarget =
        """
        INSERT INTO target (target_id, target_key)
        VALUES (@target_id, @target_key)
        ON CONFLICT(target_key) DO NOTHING;
        """;

    internal const string InsertSnapshot =
        """
        INSERT INTO snapshot (
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
        VALUES (
            (SELECT target_id FROM target WHERE target_key = @target_key),
            (SELECT IFNULL(MAX(version_number), 0) FROM snapshot WHERE target_id = @target_id),
            @target_type,
            @target_name, 
            @is_partial,
            @schema_revision,
            @software_revision,
            @export_date,
            @export_options,
            @import_date,
            @import_user,
            @import_machine,
            @source_hash,
            @source_data)
        RETURNING snapshot_id;
        """;

    internal const string InsertSnapshotInstance =
        """
        INSERT INTO snapshot_instance (snapshot_id, restored_on, restored_by)
        VALUES (@snapshot_id, @restored_on, @restored_by)
        """;

    internal const string InsertSnapshotMetadata =
        """
        INSERT INTO snapshot_property (property_id, snapshot_id, property_name, property_value)
        VALUES (@property_id, @snapshot_id, @property_name, @property_value)
        """;

    internal const string ListSnapshots =
        """
        SELECT snapshot_id [SnapshotId],
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
              source_hash [SourceHash] 
        FROM snapshot s
        JOIN target t on t.target_id = s.target_id
        WHERE @target_key is null or t.target_key = @target_key
        """;

    internal const string GetSnapshot =
        """
        SELECT 
            snapshot_id [SnapshotId],
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
        FROM snapshot s
        JOIN target t on t.target_id = s.target_id
        WHERE t.target_key = @target_key and s.version_number = @version_number
        """;

    internal const string GetLatestSnapshot =
        """
        SELECT 
            snapshot_id [SnapshotId],
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
        FROM snapshot s
        JOIN target t on t.target_id = s.target_id
        WHERE t.target_key = @target_key
        ORDER BY import_date DESC
        LIMIT 1
        """;

    internal const string DeleteTargets =
        "DELETE FROM target WHERE target_id > 0";

    internal const string PurgeTarget =
        """
        DELETE FROM snapshot_instance 
        WHERE snapshot_id IN (
            SELECT snapshot_id 
            FROM snapshot 
            WHERE target_id = (SELECT target_id FROM target WHERE target_key = @target_key)
        );

        DELETE FROM target where target_key = @target_key;
        """;

    internal const string PruneTarget =
        """
        DELETE FROM snapshot_instance 
        WHERE snapshot_id IN (
            SELECT snapshot_id 
            FROM snapshot 
            WHERE target_id = (SELECT target_id FROM target WHERE target_key = @target_key)
        )
        """;

    internal const string DeleteSnapshotByVersion =
        """
        DELETE FROM snapshot 
        WHERE target_id = (SELECT target_id FROM target WHERE target_key = @target_key)
        AND version_number = @version_number
        """;

    internal const string DeleteSnapshotsBefore =
        """
        DELETE FROM snapshot 
           WHERE (@target_key is null or target_id = (SELECT target_id FROM target where target_key = @target_key))
           AND import_date < @import_date
        """;

    /// <summary>
    /// A SQL query used to retrieve the names of all tables in the current SQLite database.
    /// It queries the SQLite system catalog to list entries of type "table" from the `sqlite_master` table.
    /// </summary>
    internal const string GetComponentTables =
        """
        SELECT name
        FROM sqlite_master
        WHERE type = 'table' 
          AND sql LIKE '%snapshot_id%' 
          AND name not in ('snapshot', 'snapshot_property')
        """;
}