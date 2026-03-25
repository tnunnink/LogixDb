using System.Data;
using LogixDb.Data.Abstractions;
using Microsoft.Data.Sqlite;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides functionality for writing a collection of <see cref="DataTable"/> objects
/// to an SQLite database using a specified connection and transaction. This class is
/// designed to handle bulk inserts into the database while maintaining transactional integrity.
/// </summary>
public class SqliteDbWriter(SqliteConnection connection, SqliteTransaction transaction) : ILogixDbWriter
{
    /// <summary>
    /// Writes a collection of <see cref="DataTable"/> objects to a database asynchronously while allowing cancellation of the operation.
    /// </summary>
    /// <param name="tables">The collection of <see cref="DataTable"/> objects to be written to the database.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
    public async Task WriteAsync(IEnumerable<DataTable> tables, CancellationToken token)
    {
        foreach (var table in tables)
        {
            await WriteTableAsync(table, token);
        }
    }

    /// <summary>
    /// Writes the data from the specified <see cref="DataTable"/> to a SQLite database asynchronously.
    /// </summary>
    /// <param name="table">The <see cref="DataTable"/> containing the data to be written to the database.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
    private async Task WriteTableAsync(DataTable table, CancellationToken token)
    {
        await using var command = new SqliteCommand(BuildInsertStatement(table), connection, transaction);

        foreach (DataColumn column in table.Columns)
            command.Parameters.Add($"@{column.ColumnName}", column.DataType.ToSqliteType());

        await command.PrepareAsync(token);

        foreach (DataRow row in table.Rows)
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
    /// Builds an SQL INSERT statement for the specified <see cref="DataTable"/>.
    /// </summary>
    /// <param name="table">The <see cref="DataTable"/> containing the schema and data to construct the INSERT statement.</param>
    /// <returns>A <see cref="string"/> representing the SQL INSERT statement for the specified table.</returns>
    private static string BuildInsertStatement(DataTable table)
    {
        var columns = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
        var parameters = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => $"@{c.ColumnName}"));

        return $"""
                INSERT INTO {table.TableName} ({columns})
                VALUES ({parameters});
                """;
    }
}