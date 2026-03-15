using System.Data;
using LogixDb.Data.Abstractions;
using Microsoft.Data.SqlClient;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Provides an abstract base class for handling the import of data into a SQL Server database
/// within the LogixDb framework. This class defines common functionality and contract adherence
/// for specific import implementations.
/// </summary>
internal abstract class SqlServerImport : ILogixDbImport
{
    /// <inheritdoc />
    public abstract Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token);

    /// <summary>
    /// Imports a collection of records into the SQL Server database using bulk copy operations
    /// for maximum performance. The method generates a data table from the records and uses
    /// SqlBulkCopy to efficiently insert all records in a single operation.
    /// </summary>
    /// <param name="records">The collection of records to import into the database.</param>
    /// <param name="map">The table mapping configuration used to generate the data table from the records.</param>
    /// <param name="session">The active database session used to get the SQL Server connection and transaction.</param>
    /// <param name="token">An optional cancellation token to observe during the operation.</param>
    /// <typeparam name="TRecord">The type of record being imported. Must be a reference type.</typeparam>
    /// <returns>A task that represents the asynchronous import operation.</returns>
    protected static async Task ImportRecords<TRecord>(
        IEnumerable<TRecord> records,
        TableMap<TRecord> map,
        ILogixDbSession session,
        CancellationToken token) where TRecord : class
    {
        var connection = session.GetConnection<SqlConnection>();
        var transaction = session.GetTransaction<SqlTransaction>();

        // Set up a bulk copy instance to insert records for max performance.
        using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, transaction);
        bulkCopy.DestinationTableName = $"dbo.{map.TableName}";

        var table = map.GenerateTable(records);
        // We need to explicitly map the column names since the table maps don't include the PK id column.
        table.Columns.Cast<DataColumn>().ToList().ForEach(c => bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName));

        await bulkCopy.WriteToServerAsync(table, token);
    }
}