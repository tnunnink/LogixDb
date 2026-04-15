namespace LogixDb.Data.SqlServer;

/// <summary>
/// Provides a static collection of SQL queries targeting a SQL Server database.
/// This class is used for common database operations such as querying snapshots
/// or deleting target entries.
/// </summary>
internal static class Sql
{
    /// <summary>
    /// A SQL query string used to ensure that a target entry exists in the database for the specified target key.
    /// If no target with the given key exists, a new target record is inserted with that key.
    /// </summary>
    internal const string EnsureTargetExists =
        """
        IF NOT EXISTS (SELECT 1 FROM target WHERE target_key = @target_key)
        BEGIN
            INSERT INTO target (target_id, target_key) VALUES (@target_id, @target_key)
        END
        """;

    /// <summary>
    /// A SQL statement used to retrieve the unique identifier (target_id) for a specific entry
    /// in the "target" table based on the provided target key. If the target key exists in the
    /// database, the corresponding target_id is returned.
    /// </summary>
    internal const string GetTargetId = "SELECT target_id FROM target WHERE target_key = @target_key";
    
    /// <summary>
    /// A SQL query that retrieves the latest version number associated with a specified target ID
    /// from the "snapshot" table. The query filters by the provided target ID to fetch the
    /// maximum version number or returns zero if no version exists.
    /// </summary>
    internal const string GetLatestVersion = "SELECT ISNULL(MAX(version_number), 0) FROM snapshot WHERE target_id = @target_id";

    /// <summary>
    /// A SQL query string used to insert a new snapshot record into the database and return the generated snapshot ID.
    /// The snapshot includes metadata such as target information, schema and software revisions, export and import details, and source data.
    /// </summary>
    internal const string InsertSnapshot =
        """
        INSERT INTO snapshot (target_id, version_number, target_type, target_name, is_partial, schema_revision, software_revision, export_date, export_options, import_date, import_user, import_machine, source_hash, source_data)
        OUTPUT INSERTED.snapshot_id
        VALUES (@target_id, @version_number, @target_type, @target_name, @is_partial, @schema_revision, @software_revision, @export_date, @export_options, @import_date, @import_user, @import_machine, @source_hash, @source_data)
        """;

    /// <summary>
    /// A SQL query string used to insert metadata properties associated with a snapshot into the snapshot_property table.
    /// Each property consists of a name-value pair linked to a specific snapshot ID.
    /// </summary>
    internal const string InsertSnapshotMetadata =
        """
        INSERT INTO snapshot_property (property_id, snapshot_id, property_name, property_value)
        VALUES (@property_id, @snapshot_id, @property_name, @property_value)
        """;

    /// <summary>
    /// A SQL query string used to retrieve a list of snapshots and their associated target details
    /// from the database. Allows filtering by a specified target key if provided.
    /// </summary>
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

    /// <summary>
    /// A SQL query string used to retrieve details of a specific snapshot associated with
    /// a given target key and version number. The query joins the `snapshot` and `target` tables
    /// to provide detailed information, including snapshot metadata and associated target
    /// attributes.
    /// </summary>
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

    /// <summary>
    /// A SQL query string used to retrieve the most recent snapshot entry from the database table "snapshot"
    /// based on the target ID associated with the specified target key.
    /// </summary>
    internal const string GetLatestSnapshot =
        """
        SELECT TOP 1 
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
        """;

    /// <summary>
    /// A SQL query string used to delete all target records from the database
    /// where the target ID is greater than zero.
    /// </summary>
    internal const string DeleteTargets = "DELETE FROM target WHERE target_id > 0";

    /// <summary>
    /// A SQL query string used to delete a specific target from the database,
    /// identified by a provided target key.
    /// </summary>
    internal const string DeleteTarget = "DELETE FROM target where target_key = @target_key ";

    /// <summary>
    /// A SQL query string used to delete a specific snapshot from the database
    /// based on the target key and version number provided.
    /// </summary>
    internal const string DeleteSnapshotByVersion =
        """
        DELETE FROM snapshot 
        WHERE target_id = (SELECT target_id FROM target WHERE target_key = @target_key)
        AND version_number = @version_number
        """;

    /// <summary>
    /// A SQL query string used to delete snapshot records from the "snapshot" table that were imported
    /// before a specified date. If a target key is provided, the deletion is further scoped to snapshots
    /// associated with the specified target key by matching it to the target_id in the "target" table.
    /// If no target key is provided, the query deletes all matching snapshots solely based on the import date.
    /// </summary>
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
    internal const string GetTableNames =
        "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
}