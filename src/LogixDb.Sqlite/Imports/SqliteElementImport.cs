using L5Sharp.Core;
using LogixDb.Core.Abstractions;
using LogixDb.Core.Common;
using Microsoft.Data.Sqlite;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Sqlite.Imports;

/// <summary>
/// Represents an import process for a specific type of Logix element into a database, using a given SQL
/// statement and table mapping for the transformation of data from an external source into the target database.
/// </summary>
/// <typeparam name="TElement">The type of Logix element being imported, constrained to types implementing
/// the <see cref="ILogixElement"/> interface.</typeparam>
/// <param name="sql">The SQL statement used for the import operation, typically an INSERT or UPDATE command.</param>
/// <param name="map">A <see cref="TableMap{TElement}"/> instance containing mappings of the fields of
/// <typeparamref name="TElement"/> to the corresponding database table columns.</param>
public abstract class SqliteElementImport<TElement>(string sql, TableMap<TElement> map) : ILogixDatabaseImport
    where TElement : ILogixElement
{
    public async Task Process(Snapshot snapshot, ILogixDatabaseSession session, CancellationToken token = default)
    {
        // Throw to roll back the parent transaction.
        token.ThrowIfCancellationRequested();

        // Retrieve the session connection and transaction
        var connection = session.GetConnection<SqliteConnection>();
        var transaction = session.GetTransaction<SqliteTransaction>();

        // Build a compiled command with configured parameters to optimize insert performance for SQLite.
        await using var command = new SqliteCommand(sql, connection, transaction);

        // We can configure snapshot id once for the entire process. 
        command.Parameters.Add(new SqliteParameter("@snapshot_id", snapshot.SnapshotId));

        // Use the map columns to prepare the parameter config dynamically.
        var columns = map.Columns.ToList();
        columns.ForEach(c => command.Parameters.Add($"@{c.Name}", c.Type.ToSqliteType()));
        command.Prepare();

        // Build a binding of the parameters to the getter function once before iteration to avoid
        // costly lookups and to make mapping explicit (by name not array index).
        var binders = columns.Select(c => (Param: command.Parameters[$"@{c.Name}"], c.Getter)).ToArray();

        // Get all source records that we would like to insert for this type.
        var records = GetRecords(snapshot.GetSource());

        foreach (var record in records)
        {
            foreach (var binder in binders)
                binder.Param.Value = binder.Getter(record) ?? DBNull.Value;

            await command.ExecuteNonQueryAsync(token);
        }
    }

    /// <summary>
    /// Retrieves a collection of records of type <typeparamref name="TElement"/> from the specified content.
    /// </summary>
    /// <param name="content">The source content from which records are retrieved.</param>
    /// <returns>A collection of records of type <typeparamref name="TElement"/>.</returns>
    protected abstract IEnumerable<TElement> GetRecords(L5X content);
}