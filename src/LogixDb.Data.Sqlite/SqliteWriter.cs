using System.Data;
using Dapper;
using LogixDb.Data.Abstractions;
using Microsoft.Data.Sqlite;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides functionality for writing a collection of <see cref="DataTable"/> objects
/// to an SQLite database using a specified connection and transaction. This class is
/// designed to handle bulk inserts into the database while maintaining transactional integrity.
/// </summary>
internal class SqliteWriter(IDbProvider provider) : IDbWriter
{
    /// <inheritdoc />
    public async Task WriteAsync(Target target, CancellationToken token)
    {
        await using var dbConnection = (SqliteConnection)await provider.OpenConnection(token);
        await using var dbTransaction = (SqliteTransaction)await dbConnection.BeginTransactionAsync(token);

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
        SqliteConnection connection,
        SqliteTransaction transaction,
        Target target
    )
    {
        // Insert target key if not already.
        var postTarget = provider.GetManagerScript(ScriptName.PostTarget);
        await connection.ExecuteAsync(postTarget, target, transaction);

        // Execute the script and retrieve both VersionId and VersionNumber
        var postVersion = provider.GetManagerScript(ScriptName.PostVersion);
        var result = await connection.QuerySingleAsync(postVersion, target, transaction);

        // Update the target version id and number that were computed from the SQL script.
        target.VersionId = (int)result.VersionId;
        target.VersionNumber = (int)result.VersionNumber;

        // Inserts all the configured metadata for the version.
        var postInfo = provider.GetManagerScript(ScriptName.PostInfo);
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
    /// Creates a temporary table in the SQLite database for the given <see cref="DataTable"/>.
    /// </summary>
    /// <param name="connection">The <see cref="SqliteConnection"/> used to connect to the SQLite database.</param>
    /// <param name="transaction">The <see cref="SqliteTransaction"/> used to ensure the operation is part of a transaction.</param>
    /// <param name="table">The <see cref="DataTable"/> representing the schema for the temporary table.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of creating the temporary table.</returns>
    private static async Task CreateTempTableAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        DataTable table,
        CancellationToken token
    )
    {
        var columns = table.Columns.Cast<DataColumn>().Select(c => $"[{c.ColumnName}] {c.DataType.ToSqliteType()}");
        var sql = $"CREATE TEMP TABLE temp_{table.TableName} ({string.Join(", ", columns)});";
        await using var command = new SqliteCommand(sql, connection, transaction);
        await command.ExecuteNonQueryAsync(token);
    }

    /// <summary>
    /// Writes the data from the specified <see cref="DataTable"/> into the corresponding temporary table
    /// in the SQLite database within the context of the provided connection and transaction.
    /// </summary>
    /// <param name="connection">The <see cref="SqliteConnection"/> used to connect to the SQLite database.</param>
    /// <param name="transaction">The <see cref="SqliteTransaction"/> that ensures the operation is executed within an active transaction.</param>
    /// <param name="table">The <see cref="DataTable"/> containing the data to be written to the temporary table.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to cancel the asynchronous operation if required.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of writing the data to the temporary table.</returns>
    private static async Task WriteTableAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        DataTable table,
        CancellationToken token)
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
    /// Executes a merge operation in the SQLite database for the specified table using a predefined merge script.
    /// </summary>
    /// <param name="connection">The <see cref="SqliteConnection"/> used to connect to the SQLite database.</param>
    /// <param name="transaction">The <see cref="SqliteTransaction"/> used to ensure the operation is part of a transaction.</param>
    /// <param name="versionId">The version identifier to associate with the merged data.</param>
    /// <param name="table">The <see cref="DataTable"/> containing the data to be merged.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of executing the merge.</returns>
    private async Task ExecuteMergeAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int versionId,
        DataTable table,
        CancellationToken token)
    {
        var script = provider.GetMergeScript(table.TableName);
        await using var command = new SqliteCommand(script, connection, transaction);
        command.Parameters.AddWithValue("@VersionId", versionId);
        await command.ExecuteNonQueryAsync(token);
    }
}