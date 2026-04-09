using System.Data;
using LogixDb.Data.Abstractions;
using Microsoft.Data.SqlClient;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Implements a writer for persisting <see cref="DataTable"/> objects to a SQL Server database
/// using an established <see cref="SqlConnection"/> and optional <see cref="SqlTransaction"/>.
/// </summary>
internal class SqlServerDbWriter(SqlDbSession session) : ILogixDbWriter
{
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
            await WriteTableAsync(table, token);
        }
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
        using var bulkCopy = new SqlBulkCopy(session.Connection, SqlBulkCopyOptions.KeepIdentity, session.Transaction);
        bulkCopy.DestinationTableName = $"dbo.{table.TableName}";
        
        // Increase timeout to handle large files (0 = infinite)
        bulkCopy.BulkCopyTimeout = 0;

        // We need to explicitly map the column names since the table maps don't include the PK id column.
        table.Columns.Cast<DataColumn>().ToList().ForEach(c => bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName));

        await bulkCopy.WriteToServerAsync(table, token);
    }
}