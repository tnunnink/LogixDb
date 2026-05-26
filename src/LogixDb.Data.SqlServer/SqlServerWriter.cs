using System.Data;
using LogixDb.Data.Abstractions;
using Microsoft.Data.SqlClient;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Implements a writer for persisting <see cref="DataTable"/> objects to a SQL Server database
/// using an established <see cref="SqlConnection"/> and optional <see cref="SqlTransaction"/>.
/// </summary>
internal class SqlServerWriter(int versionId, SqlConnection connection, SqlTransaction transaction) : IDbWriter
{
    /// <summary>
    /// A dictionary mapping table names to corresponding SQL Server merge scripts.
    /// </summary>
    private static readonly Dictionary<string, string> MergeScripts = new()
    {
        { "controller", SqlServerScript.MergeController },
        { "data_type", SqlServerScript.MergeDataType },
        { "data_type_member", SqlServerScript.MergeDataTypeMember },
        { "aoi", SqlServerScript.MergeAoi },
        { "aoi_parameter", SqlServerScript.MergeAoiParameter },
        { "aoi_rung", SqlServerScript.MergeAoiRung },
        { "operand", SqlServerScript.MergeOperand },
        { "module", SqlServerScript.MergeModule },
        { "task", SqlServerScript.MergeTask },
        { "program", SqlServerScript.MergeProgram },
        { "routine", SqlServerScript.MergeRoutine },
        { "rung", SqlServerScript.MergeRung },
        { "rung_instruction", SqlServerScript.MergeRungInstruction },
        { "rung_argument", SqlServerScript.MergeRungArgument },
        { "rung_reference", SqlServerScript.MergeRungReference },
        { "tag", SqlServerScript.MergeTag },
        { "tag_member", SqlServerScript.MergeTagMember },
        { "tag_comment", SqlServerScript.MergeTagComment },
        { "tag_producer", SqlServerScript.MergeTagProducer },
        { "tag_consumer", SqlServerScript.MergeTagConsumer },
        { "tag_value", SqlServerScript.MergeTagValue }
    };

    /// <summary>
    /// Writes the specified collection of <see cref="DataTable"/> objects to the SQL Server database asynchronously.
    /// </summary>
    /// <param name="tables">The collection of <see cref="DataTable"/> objects to be written to the database.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to observe cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
    /// Creates a local temporary table in the SQL Server database corresponding to the structure of the given <see cref="DataTable"/> asynchronously.
    /// </summary>
    private async Task CreateTempTableAsync(DataTable table, CancellationToken token)
    {
        var columns = table.Columns.Cast<DataColumn>().Select(c => $"[{c.ColumnName}] {c.DataType.ToSqlServerType()}");
        var sql = $"CREATE TABLE #temp_{table.TableName} ({string.Join(", ", columns)});";
        await using var command = new SqlCommand(sql, connection, transaction);
        await command.ExecuteNonQueryAsync(token);
    }

    /// <summary>
    /// Writes the specified <see cref="DataTable"/> to the SQL Server database asynchronously.
    /// </summary>
    /// <param name="table">The <see cref="DataTable"/> to be written to the database.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to observe cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task WriteTableAsync(DataTable table, CancellationToken token)
    {
        // Set up a bulk copy instance to insert records for max performance.
        using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
        bulkCopy.DestinationTableName = $"#temp_{table.TableName}";
        bulkCopy.BulkCopyTimeout = 0;
        bulkCopy.BatchSize = 10000;

        // We need to explicitly map the column names since the table maps don't include the PK id column.
        table.Columns.Cast<DataColumn>().ToList().ForEach(c => bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName));

        await bulkCopy.WriteToServerAsync(table, token);
    }

    /// <summary>
    /// Executes the merge script for the specified <see cref="DataTable"/> in the context of a SQL Server database.
    /// </summary>
    private async Task ExecuteMergeAsync(DataTable table, CancellationToken token)
    {
        if (!MergeScripts.TryGetValue(table.TableName, out var script))
            return;

        await using var command = new SqlCommand(script, connection, transaction);
        command.Parameters.AddWithValue("@VersionId", versionId);
        command.CommandTimeout = 0;
        await command.ExecuteNonQueryAsync(token);
    }
}