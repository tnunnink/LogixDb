using System.Data;
using Dapper;
using LogixDb.Data.Abstractions;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// A class responsible for processing and importing snapshot data into an SQLite database.
/// Implements the <see cref="ILogixDbImport"/> interface to define the import behavior.
/// </summary>
internal class SqliteSnapshotImport : ILogixDbImport
{
    private const string EnsureTargetExists =
        """
        INSERT INTO target (target_key)
        VALUES (@target_key)
        ON CONFLICT(target_key) DO NOTHING;
        """;

    private const string GetTargetId =
        """
        SELECT target_id
        FROM target
        WHERE target_key = @target_key;
        """;

    private const string InsertSnapshot =
        """
        INSERT INTO snapshot (target_id, target_type, target_name, is_partial, schema_revision, software_revision, export_date, export_options, import_date, import_user, import_machine, source_hash, source_data) 
        VALUES (@target_id, @target_type, @target_name, @is_partial, @schema_revision, @software_revision, @export_date, @export_options, @import_date, @import_user, @import_machine, @source_hash, @source_data)
        RETURNING snapshot_id;
        """;

    private const string InsertProperties =
        """
        INSERT INTO snapshot_property (snapshot_id, property_name, property_value)
        VALUES (@snapshot_id, @property_name, @property_value)
        """;

    public async Task Process(Snapshot snapshot, ILogixDbSession session, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var connection = session.GetConnection<IDbConnection>();
        var transaction = session.GetTransaction<IDbTransaction>();

        // Ensure the target entry exists and get the corresponding target id to use for the snapshot insert.
        var key = new { target_key = snapshot.TargetKey };
        await connection.ExecuteAsync(EnsureTargetExists, key, transaction);
        var targetId = await connection.QuerySingleAsync<int>(GetTargetId, key, transaction);

        // Post the provided snapshot to the database. Update the snapshot instance with the inserted ID.
        snapshot.SnapshotId = await connection.ExecuteScalarAsync<int>(InsertSnapshot, new
        {
            target_id = targetId,
            target_type = snapshot.TargetType,
            target_name = snapshot.TargetName,
            is_partial = snapshot.IsPartial,
            schema_revision = snapshot.SchemaRevision,
            software_revision = snapshot.SoftwareRevision,
            export_date = snapshot.ExportDate,
            export_options = snapshot.ExportOptions,
            import_date = snapshot.ImportDate,
            import_user = snapshot.ImportUser,
            import_machine = snapshot.ImportMachine,
            source_hash = snapshot.SourceHash,
            source_data = snapshot.SourceData
        }, transaction);

        // Post the snapshot metadata to the database.
        await connection.ExecuteAsync(InsertProperties,
            snapshot.Metadata.Select(p => new
            {
                snapshot_id = snapshot.SnapshotId,
                property_name = p.Key,
                property_value = p.Value
            }).ToList(),
            transaction);
    }
}