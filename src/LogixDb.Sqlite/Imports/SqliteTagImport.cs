using JetBrains.Annotations;
using L5Sharp.Core;
using LogixDb.Core.Abstractions;
using LogixDb.Core.Common;
using LogixDb.Core.Maps;
using Microsoft.Data.Sqlite;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Sqlite.Imports;

/// <summary>
/// Represents a class for importing tag data into a SQLite database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import tags into a SQLite database
/// by using a specific set of preconfigured SQL commands and mappings. It works in
/// conjunction with a parent transaction to ensure atomic operations are performed safely.
/// </remarks>
[UsedImplicitly]
public class SqliteTagImport(SqliteConnection connection, SqliteTransaction transaction) : ILogixDatabaseImport
{
    /// <summary>
    /// A static instance of the <see cref="TagMap"/> class, used to map columns between
    /// the database table and the corresponding properties in the <see cref="Tag"/> model.
    /// </summary>
    private static readonly TagMap Map = new();

    private const string InsertTags =
        """
        INSERT INTO tag (snapshot_id, refereence, base_name, tag_name)
        VALUES (@snapshot_id, @refereence, @base_name, @tag_name)
        """;

    /// <inheritdoc />
    public async Task Process(Snapshot snapshot, CancellationToken token = default)
    {
        // Throw to roll back the parent transaction.
        token.ThrowIfCancellationRequested();

        // Build a compiled command with configured parameters to optimize insert performance for SQLite.
        await using var command = new SqliteCommand(InsertTags, connection, transaction);

        // We can configure snapshot id once for the entire process. 
        command.Parameters.Add(new SqliteParameter("@snapshot_id", snapshot.SnapshotId));

        // Use the map columns to prepare the parameter config dynamically.
        var columns = Map.Columns.ToList();
        columns.ForEach(c => command.Parameters.Add($"@{c.Name}", c.Type.ToSqliteType()));
        command.Prepare();

        // Build a binding of the parameters to the getter function once before iteration to avoid
        // costly lookups and to make mapping explicit (by name not array index).
        var binders = columns.Select(c => (Param: command.Parameters[$"@{c.Name}"], c.Getter)).ToArray();

        // Get all source records that we would like to insert for this type.
        var source = snapshot.GetSource();
        var records = source.Query<Tag>().SelectMany(t => t.Members()).ToList();

        foreach (var record in records)
        {
            foreach (var binder in binders)
                binder.Param.Value = binder.Getter(record) ?? DBNull.Value;

            await command.ExecuteNonQueryAsync(token);
        }
    }
}