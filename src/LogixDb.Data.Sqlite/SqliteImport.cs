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
    /// Builds and prepares a parameterized SQLite command for inserting records into the database.
    /// The command is configured with parameters corresponding to the table columns defined in the mapping.
    /// </summary>
    /// <param name="map">The table mapping configuration that defines the columns and their types for the command.</param>
    /// <param name="session">The active database session used to get the SQLite connection and transaction.</param>
    /// <returns>A prepared SQLite command ready for execution with parameterized insert operations.</returns>
    protected static SqliteCommand BuildCommand<TRecord>(TableMap<TRecord> map, ILogixDbSession session)
        where TRecord : class
    {
        var connection = session.GetConnection<SqliteConnection>();
        var transaction = session.GetTransaction<SqliteTransaction>();

        var command = new SqliteCommand(BuildInsertStatement(map), connection, transaction);
        var columns = map.Columns.ToList();
        columns.ForEach(c => command.Parameters.Add($"@{c.Name}", c.Type.ToSqliteType()));
        command.Prepare();

        return command;
    }

    /// <summary>
    /// Imports a collection of records into the SQLite database by generating a data table
    /// from the records and executing the provided command for each row.
    /// </summary>
    /// <param name="records">The collection of records to import into the database.</param>
    /// <param name="map">The table mapping configuration used to generate the data table from the records.</param>
    /// <param name="command">The prepared SQLite command used to execute the insert operations.</param>
    /// <param name="token">An optional cancellation token to observe during the operation.</param>
    /// <typeparam name="TRecord">The type of record being imported. Must be a reference type.</typeparam>
    /// <returns>A task that represents the asynchronous import operation.</returns>
    protected static async Task ImportRecords<TRecord>(
        IEnumerable<TRecord> records,
        TableMap<TRecord> map,
        SqliteCommand command,
        CancellationToken token) where TRecord : class
    {
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