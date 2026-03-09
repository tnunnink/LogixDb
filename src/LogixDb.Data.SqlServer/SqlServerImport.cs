using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using Microsoft.Data.SqlClient;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Represents an abstract base class to handle the import of <typeparamref name="TElement"/> elements into a SQL Server database.
/// The process is driven by mapping the elements to database tables and using bulk copy operations.
/// </summary>
/// <typeparam name="TElement">
/// The type of element to be imported, which must implement the <see cref="ILogixElement"/> interface.
/// </typeparam>
internal abstract class SqlServerImport<TElement>(TableMap<TElement> map) : ILogixDbImport where TElement : class
{
    /// <summary>
    /// Represents the mapping configuration between a given data entity type and a SQL Server table.
    /// Provides the structural definition and functionality needed for bulk import operations,
    /// including the table name, column mappings, and helper methods for generating database-compatible data.
    /// </summary>
    protected readonly TableMap<TElement> Map = map;

    /// <summary>
    /// Executes the import process, transferring records from the provided snapshot to the SQL Server database using bulk copy.
    /// </summary>
    /// <param name="snapshot">The snapshot containing the source data to be imported.</param>
    /// <param name="session">The database session providing access to the connection and transaction.</param>
    /// <param name="token">The cancellation token used to observe cancellation requests during the import process.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="NotImplementedException">Thrown if a required method or implementation is missing.</exception>
    public async Task Process(Snapshot snapshot, ILogixDbSession session, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var connection = session.GetConnection<SqlConnection>();
        var transaction = session.GetTransaction<SqlTransaction>();

        // Set up a bulk copy instance to insert records for max performance.
        using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, transaction);
        bulkCopy.DestinationTableName = $"dbo.{Map.TableName}";

        var records = Map.GetRecords(snapshot);
        var table = Map.GenerateTable(records);
        // We need to explicitly map the column names since the table maps don't include the PK id column.
        table.Columns.Cast<DataColumn>().ToList().ForEach(c => bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName));

        await bulkCopy.WriteToServerAsync(table, token);
    }
}