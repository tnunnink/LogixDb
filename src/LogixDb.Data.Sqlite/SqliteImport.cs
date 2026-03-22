using System.Data;
using LogixDb.Data.Abstractions;
using Microsoft.Data.Sqlite;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides an abstract base class for handling the import of data into an SQLite database
/// within the LogixDb framework. This class defines common functionality and contract adherence
/// for specific import implementations.
/// </summary>
public abstract class SqliteImport : ILogixDbImport
{
    /// <inheritdoc />
    public abstract Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token);

    /// <summary>
    /// Imports a collection of records into the database using the specified table mapping, session, and cancellation token.
    /// </summary>
    /// <typeparam name="TRecord">The type of records to be imported. The type must be a class.</typeparam>
    /// <param name="records">The collection of records to be imported.</param>
    /// <param name="map">The table mapping used to define the structure of the database table.</param>
    /// <param name="session">The database session used to access the underlying database connection.</param>
    /// <param name="token">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation of importing records into the database.</returns>
    protected static async Task ImportRecords<TRecord>(
        IEnumerable<TRecord> records,
        TableMap<TRecord> map,
        ILogixDbSession session,
        CancellationToken token) where TRecord : class
    {
        var connection = session.GetConnection<SqliteConnection>();
        var transaction = session.GetTransaction<SqliteTransaction>();

        await using var command = new SqliteCommand(BuildInsertStatement(map), connection, transaction);
        map.Columns.ToList().ForEach(c => command.Parameters.Add($"@{c.Name}", c.Type.ToSqliteType()));
        command.Prepare();

        var dataTable = map.GenerateTable(records);

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
    private static string BuildInsertStatement<TRecord>(TableMap<TRecord> map) where TRecord : class
    {
        var columns = string.Join(", ", map.Columns.Select(c => c.Name));
        var parameters = string.Join(", ", map.Columns.Select(c => $"@{c.Name}"));

        return $"""
                INSERT INTO {map.TableName} ({columns})
                VALUES ({parameters});
                """;
    }
}