using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using Microsoft.Data.Sqlite;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides an abstract base class for importing database elements into an SQLite database.
/// This class maps elements of type <typeparamref name="TRecord"/> to SQLite database tables
/// using the provided table mapping and performs optimized bulk insertion of data.
/// </summary>
/// <typeparam name="TRecord">
/// The type of element being imported. This type must implement the <see cref="ILogixElement"/> interface.
/// </typeparam>
public abstract class SqliteImport<TRecord>(TableMap<TRecord> map) : ILogixDbImport where TRecord : class
{
    /// <summary>
    /// Represents the mapping configuration between a data model and a database table.
    /// The variable is used to define and manage the mapping of columns, parameters, and
    /// other related operations necessary for database interactions.
    /// </summary>
    protected readonly TableMap<TRecord> Map = map;

    /// <summary>
    /// Processes a snapshot and inserts data into the associated SQLite database session.
    /// </summary>
    /// <param name="snapshot">The snapshot containing the source data to be processed.</param>
    /// <param name="session">The active database session used for SQLite operations.</param>
    /// <param name="token">An optional cancellation token to observe during the operation.</param>
    /// <returns>A task that represents the asynchronous processing operation.</returns>
    public async Task Process(Snapshot snapshot, ILogixDbSession session, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var connection = session.GetConnection<SqliteConnection>();
        var transaction = session.GetTransaction<SqliteTransaction>();

        await using var command = new SqliteCommand(BuildInsertStatement(Map), connection, transaction);
        var columns = Map.Columns.ToList();
        columns.ForEach(c => command.Parameters.Add($"@{c.Name}", c.Type.ToSqliteType()));
        command.Prepare();

        var dataTable = GetData(snapshot);

        foreach (DataRow row in dataTable.Rows)
        {
            for (var i = 0; i < command.Parameters.Count; i++)
            {
                var parameter = command.Parameters[i];
                parameter.Value = row[i];
            }

            await command.ExecuteNonQueryAsync(token);
        }
    }

    /// <summary>
    /// Retrieves data from the provided snapshot and returns it as a DataTable.
    /// </summary>
    /// <param name="snapshot">The snapshot containing the source data to be processed.</param>
    /// <returns>A DataTable populated with the data extracted from the snapshot.</returns>
    protected abstract DataTable GetData(Snapshot snapshot);

    /// <summary>
    /// Constructs an SQL INSERT statement for a table defined by the implementing TableMap.
    /// The statement includes named parameters corresponding to the table's columns and
    /// additional fields such as a snapshot ID and record hash.
    /// </summary>
    /// <returns>
    /// A string representing the SQL INSERT statement including column names and parameters.
    /// The resulting statement follows the format:
    /// "INSERT INTO {TableName} (snapshot_id, {column names}, record_hash)
    /// VALUES (@snapshot_id, {parameters}, @record_hash);"
    /// </returns>
    private static string BuildInsertStatement(TableMap<TRecord> map)
    {
        var columns = string.Join(", ", map.Columns.Select(c => c.Name));
        var parameters = string.Join(", ", map.Columns.Select(c => $"@{c.Name}"));

        return $"""
                INSERT INTO {map.TableName} ({columns})
                VALUES ({parameters});
                """;
    }
}