using System.Data;
using LogixDb.Data.Abstractions;
using Microsoft.Data.Sqlite;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides functionality for writing a collection of <see cref="DataTable"/> objects
/// to an SQLite database using a specified connection and transaction. This class is
/// designed to handle bulk inserts into the database while maintaining transactional integrity.
/// </summary>
internal class SqliteWriter(int versionId, SqliteConnection connection, SqliteTransaction transaction) : IDbWriter
{
    /// <summary>
    /// A dictionary mapping table names to corresponding SQLite merge scripts.
    /// The keys represent the names of database tables, and the values are
    /// the SQL merge scripts that will be executed during data processing.
    /// This dictionary is used to dynamically retrieve and apply merge scripts
    /// for tables during database write operations, ensuring efficient and
    /// consistent data updates.
    /// </summary>
    private static readonly Dictionary<string, string> MergeScripts = new()
    {
        { "controller", SqliteScript.MergeController },
        { "data_type", SqliteScript.MergeDataType },
        { "data_type_member", SqliteScript.MergeDataTypeMember },
        { "aoi", SqliteScript.MergeAoi },
        { "aoi_parameter", SqliteScript.MergeAoiParameter },
        { "aoi_rung", SqliteScript.MergeAoiRung },
        { "operand", SqliteScript.MergeOperand },
        { "module", SqliteScript.MergeModule },
        { "task", SqliteScript.MergeTask },
        { "program", SqliteScript.MergeProgram },
        { "routine", SqliteScript.MergeRoutine },
        { "rung", SqliteScript.MergeRung },
        { "instruction", SqliteScript.MergeInstruction },
        { "argument", SqliteScript.MergeArgument },
        { "tag", SqliteScript.MergeTag },
        { "tag_member", SqliteScript.MergeTagMember },
        { "tag_comment", SqliteScript.MergeTagComment },
        { "tag_producer", SqliteScript.MergeTagProducer },
        { "tag_consumer", SqliteScript.MergeTagConsumer },
        { "tag_value", SqliteScript.MergeTagValue }
    };

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
            await CreateTempTableAsync(table, token);
            await WriteTableAsync(table, token);
            await ExecuteMergeAsync(table, token);
        }
    }

    /// <summary>
    /// Creates a temporary table in the database corresponding to the structure of the given <see cref="DataTable"/> asynchronously.
    /// This operation is performed within the specified connection and transaction.
    /// </summary>
    /// <param name="table">The <see cref="DataTable"/> whose structure will be used to define the temporary table.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of creating the temporary table.</returns>
    private async Task CreateTempTableAsync(DataTable table, CancellationToken token)
    {
        var columns = table.Columns.Cast<DataColumn>().Select(c => $"[{c.ColumnName}] {c.DataType.ToSqliteType()}");
        var sql = $"CREATE TEMP TABLE temp_{table.TableName} ({string.Join(", ", columns)});";
        await using var command = new SqliteCommand(sql, connection, transaction);
        await command.ExecuteNonQueryAsync(token);
    }

    /// <summary>
    /// Writes the data from the specified <see cref="DataTable"/> to a SQLite database asynchronously.
    /// </summary>
    /// <param name="table">The <see cref="DataTable"/> containing the data to be written to the database.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
    private async Task WriteTableAsync(DataTable table, CancellationToken token)
    {
        var columns = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
        var parameters = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => $"@{c.ColumnName}"));
        var sql = $"INSERT INTO temp_{table.TableName} ({columns}) VALUES ({parameters});";

        await using var command = new SqliteCommand(sql, connection, transaction);

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
    /// Executes the merge script for the specified <see cref="DataTable"/> in the context of an SQLite database.
    /// </summary>
    /// <param name="table">The <see cref="DataTable"/> containing the data to be merged into the database.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous execution of the merge script.</returns>
    private async Task ExecuteMergeAsync(DataTable table, CancellationToken token)
    {
        if (!MergeScripts.TryGetValue(table.TableName, out var script))
            return;

        await using var command = new SqliteCommand(script, connection, transaction);
        command.Parameters.AddWithValue("@VersionId", versionId);
        await command.ExecuteNonQueryAsync(token);
    }
}