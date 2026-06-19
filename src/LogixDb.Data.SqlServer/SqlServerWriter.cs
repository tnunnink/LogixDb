using System.Data;
using Dapper;
using LogixDb.Data.Abstractions;
using Microsoft.Data.SqlClient;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Implements a writer for persisting <see cref="DataTable"/> objects to a SQL Server database
/// using an established <see cref="SqlConnection"/> and optional <see cref="SqlTransaction"/>.
/// </summary>
internal class SqlServerWriter(IDbProvider provider) : IDbWriter
{
    /// <inheritdoc />
    public async Task WriteAsync(Target target, CancellationToken token)
    {
        await using var dbConnection = (SqlConnection)await provider.OpenConnection(token);
        await using var dbTransaction = (SqlTransaction)await dbConnection.BeginTransactionAsync(token);

        try
        {
            // Before writing to the database, ensure we can parse and compile the L5X into a set of data tables.
            var tables = target.Compile().ToList();

            // Start with database import by inserting new target and version records.
            // This sets the local version id and number which we need to merge each compiled table. 
            await PostTargetVersionAsync(dbConnection, dbTransaction, target);

            // Bulk copy import - create temp tables, load data, and execute the merge script for each compiled table.
            foreach (var table in tables)
            {
                await CreateTempTableAsync(dbConnection, dbTransaction, table, token);
                await WriteTableAsync(dbConnection, dbTransaction, table, token);
                await ExecuteMergeAsync(dbConnection, dbTransaction, target.VersionId, table, token);
            }

            // Commit once complete if now issues.
            await dbTransaction.CommitAsync(token);
        }
        catch (Exception)
        {
            // Roll back the entire process for any error and throw the exception to bubble up to the caller.
            await dbTransaction.RollbackAsync(token);
            throw;
        }
    }

    /// <summary>
    /// Posts the target version and its associated metadata into the database using the provided SQL scripts.
    /// </summary>
    /// <param name="connection">The SQLite database connection to be used for the operation.</param>
    /// <param name="transaction">The SQLite transaction in which this operation should be executed.</param>
    /// <param name="target">The target object containing version and metadata information to be persisted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task PostTargetVersionAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        Target target
    )
    {
        // Insert target key if not already.
        var postTarget = provider.GetScript(ScriptName.PostTarget);
        await connection.ExecuteAsync(postTarget, target, transaction);

        // Execute the script and retrieve both VersionId and VersionNumber
        var postVersion = provider.GetScript(ScriptName.PostVersion);
        var result = await connection.QuerySingleAsync(postVersion, target, transaction);

        // Update the target version id and number that were computed from the SQL script.
        target.VersionId = (int)result.VersionId;
        target.VersionNumber = (int)result.VersionNumber;

        // Inserts all the configured metadata for the version.
        var postInfo = provider.GetScript(ScriptName.PostInfo);
        await connection.ExecuteAsync(postInfo,
            target.Info.Select(p => new
            {
                property_id = Guid.CreateVersion7(),
                version_id = target.VersionId,
                property_name = p.Key,
                property_value = p.Value
            }).ToList(),
            transaction
        );
    }

    /// <summary>
    /// Creates a temporary table in the SQL Server database based on the structure of the provided <see cref="DataTable"/>.
    /// </summary>
    /// <param name="connection">The active <see cref="SqlConnection"/> used to execute the SQL command.</param>
    /// <param name="transaction">The active <see cref="SqlTransaction"/> within which the command should be executed.</param>
    /// <param name="table">The <see cref="DataTable"/> whose schema is used to define the temporary table.</param>
    /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of creating the temporary table.</returns>
    private static async Task CreateTempTableAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        DataTable table,
        CancellationToken token
    )
    {
        var columns = table.Columns.Cast<DataColumn>().Select(c => $"[{c.ColumnName}] {c.DataType.ToSqlServerType()}");
        var sql = $"CREATE TABLE #temp_{table.TableName} ({string.Join(", ", columns)});";
        await using var command = new SqlCommand(sql, connection, transaction);
        await command.ExecuteNonQueryAsync(token);
    }

    /// <summary>
    /// Writes the contents of the specified <see cref="DataTable"/> to a temporary table in the database
    /// using a bulk copy mechanism for high-performance data insertion.
    /// </summary>
    /// <param name="connection">The <see cref="SqlConnection"/> to the database where the data will be written.</param>
    /// <param name="transaction">The <see cref="SqlTransaction"/> associated with the operation, ensuring atomicity and consistency.</param>
    /// <param name="table">The <see cref="DataTable"/> containing the data to be written to the temporary table.</param>
    /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private static async Task WriteTableAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        DataTable table,
        CancellationToken token
    )
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
    /// Executes a SQL merge operation for the specified <see cref="DataTable"/> against a target database table.
    /// </summary>
    /// <param name="connection">The active <see cref="SqlConnection"/> to the database where the merge will be executed.</param>
    /// <param name="transaction">The active <see cref="SqlTransaction"/> to ensure changes are part of a transaction.</param>
    /// <param name="versionId">The version identifier associated with the data being merged.</param>
    /// <param name="table">The <see cref="DataTable"/> containing the data to be merged.</param>
    /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    private async Task ExecuteMergeAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int versionId,
        DataTable table,
        CancellationToken token)
    {
        var script = provider.GetMergeScript(table.TableName);
        await using var command = new SqlCommand(script, connection, transaction);
        command.Parameters.AddWithValue("@VersionId", versionId);
        await command.ExecuteNonQueryAsync(token);
    }
}