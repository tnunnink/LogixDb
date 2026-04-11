namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides a collection of SQL query strings used for interacting with a SQLite database
/// in the "snapshot" and related tables.
/// </summary>
internal static class SqlStatement
{
    /// <summary>
    /// A SQL statement used to ensure the existence of a target entry in the "target" table by inserting a new record
    /// with the specified target key. If a record with the same target key already exists, the operation does nothing,
    /// preventing duplicate entries for the key.
    /// </summary>
    internal const string EnsureTargetExists =
        """
        INSERT INTO target (target_id, target_key)
        VALUES (@target_id, @target_key)
        ON CONFLICT(target_key) DO NOTHING;
        """;

    /// <summary>
    /// A SQL statement used to retrieve the unique identifier (target_id) for a specific entry
    /// in the "target" table based on the provided target key. If the target key exists in the
    /// database, the corresponding target_id is returned.
    /// </summary>
    internal const string GetTargetId = "SELECT target_id FROM target WHERE target_key = @target_key";

    /// <summary>
    /// A SQL statement used to insert a new snapshot record into the "snapshot" table.
    /// This statement populates various fields such as target details, schema and software revisions,
    /// export and import metadata, as well as source hash and data. Upon successful insertion, the
    /// statement returns the unique identifier (snapshot_id) for the newly created snapshot.
    /// </summary>
    internal const string InsertSnapshot =
        """
        INSERT INTO snapshot (target_id, target_type, target_name, is_partial, schema_revision, software_revision, export_date, export_options, import_date, import_user, import_machine, source_hash, source_data) 
        VALUES (@target_id, @target_type, @target_name, @is_partial, @schema_revision, @software_revision, @export_date, @export_options, @import_date, @import_user, @import_machine, @source_hash, @source_data)
        RETURNING snapshot_id;
        """;

    /// <summary>
    /// A SQL statement that inserts metadata for a snapshot into the "snapshot_property" table.
    /// It includes the snapshot ID, property name, and property value as parameters, allowing the
    /// association of specific metadata properties with a given snapshot.
    /// </summary>
    internal const string InsertSnapshotMetadata =
        """
        INSERT INTO snapshot_property (property_id, snapshot_id, property_name, property_value)
        VALUES (@property_id, @snapshot_id, @property_name, @property_value)
        """;

    /// <summary>
    /// A SQL query string used to retrieve a list of snapshot entries from the database table "snapshot."
    /// The query supports filtering snapshots based on the associated target key by matching it with the
    /// target_id retrieved from the "target" table. If no target key is specified, all snapshots are returned.
    /// </summary>
    internal const string ListSnapshots =
        """
        SELECT snapshot_id [SnapshotId],
              t.target_id [TargetId],
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
    /// A SQL query string used to fetch a snapshot entry from the database table "snapshot"
    /// where the snapshot ID matches the specified parameter.
    /// </summary>
    internal const string GetSnapshotById =
        """
        SELECT snapshot_id [SnapshotId],
              t.target_id [TargetId],
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
        WHERE snapshot_id = @snapshot_id
        """;

    /// <summary>
    /// A SQL query string used to retrieve the most recent snapshot entry from the database table "snapshot"
    /// based on the target ID associated with the specified target key.
    /// </summary>
    internal const string GetLatestSnapshot =
        """
        SELECT snapshot_id [SnapshotId],
              t.target_id [TargetId],
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

    /// <summary>
    /// A SQL statement used to retrieve the most recent snapshot ID associated with a specific target key.
    /// The query joins the "snapshot" and "target" tables to identify the relevant record, orders the
    /// results by the import date in descending order, and limits the output to a single record.
    /// </summary>
    internal const string GetLatestSnapshotId =
        """
        SELECT snapshot_id 
        FROM snapshot s
        JOIN target t on t.target_id = s.target_id
        WHERE t.target_key = @target_key
        ORDER BY import_date DESC
        LIMIT 1
        """;

    /// <summary>
    /// A SQL statement defined to delete all related content associated with a specific snapshot identifier.
    /// This operation removes entries from multiple tables, including "controller", "data_type", "aoi", "module",
    /// "tag", "program", "task", and "operand", where the "snapshot_id" matches the given parameter.
    /// It is used to ensure that no residual data remains for the specified snapshot in the database.
    /// </summary>
    internal const string DeleteSnapshotContent =
        """
        DELETE FROM controller WHERE snapshot_id = @snapshot_id;
        DELETE FROM data_type WHERE snapshot_id = @snapshot_id;
        DELETE FROM aoi WHERE snapshot_id = @snapshot_id;
        DELETE FROM module WHERE snapshot_id = @snapshot_id;
        DELETE FROM tag WHERE snapshot_id = @snapshot_id;
        DELETE FROM program WHERE snapshot_id = @snapshot_id;
        DELETE FROM task WHERE snapshot_id = @snapshot_id;
        DELETE FROM operand WHERE snapshot_id = @snapshot_id;
        """;

    /// <summary>
    /// A SQL query string used to delete all entries from the "target" database table.
    /// This query ensures that every record in the table where the target_id is greater than zero is removed,
    /// effectively clearing the table of all targets.
    /// </summary>
    internal const string DeleteAllTargets = "DELETE FROM target WHERE target_id > 0";

    /// <summary>
    /// A SQL query string used to delete a target entry from the "target" table in the database.
    /// The deletion is performed by matching the target_key value with the key provided via the @target_key parameter.
    /// </summary>
    internal const string DeleteTargetById = "DELETE FROM target where target_key = @target_key ";

    /// <summary>
    /// A SQL query string designed to delete a specific snapshot entry from the database
    /// table "snapshot" based on the provided snapshot ID. This query ensures the removal
    /// of the snapshot record identified by the "snapshot_id" parameter.
    /// </summary>
    internal const string DeleteSnapshotById = "DELETE FROM snapshot WHERE snapshot_id = @snapshot_id;";

    /// <summary>
    /// A SQL query string used to delete the latest snapshot entry from the "snapshot" table, determined by the most recent
    /// import_date. The query optionally filters snapshots by a specified target key, matching it to the target_id from
    /// the "target" table. If no target key is provided, the latest snapshot is deleted regardless of its associated target.
    /// </summary>
    internal const string DeleteSnapshotByLatest =
        """
        DELETE FROM snapshot 
        WHERE snapshot_id = (
            SELECT snapshot_id 
            FROM snapshot 
            WHERE (@target_key IS NULL OR target_id = (SELECT target_id FROM target WHERE target_key = @target_key))
            ORDER BY import_date DESC 
            LIMIT 1
        )
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
        """
        SELECT name 
        FROM sqlite_master 
        WHERE type = 'table' AND name IN @components
        """;
}