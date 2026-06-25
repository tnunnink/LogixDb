using Dapper;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using Microsoft.Data.Sqlite;

namespace LogixDb.Testing.Sqlite;

/// <summary>
/// Represents a temporary SQLite database designed for use in testing scenarios.
/// Provides functionality to create, manage, and purge test database instances.
/// </summary>
public sealed class SqliteTestDatabase : IDisposable
{
    /// <summary>
    /// The file path for the temporary SQLite database used during the test execution.
    /// This path is dynamically generated to ensure uniqueness for each test run.
    /// </summary>
    private readonly string _tempDb = Path.Combine(Path.GetTempPath(), $"Logix_{Guid.NewGuid():N}.db");

    /// <summary>
    /// Represents the connection details used to interact with the temporary SQLite database
    /// within the context of the test framework. This property provides information about
    /// the database provider and file path used for the test database.
    /// </summary>
    public DbConnectionInfo Connection { get; private set; } = null!;

    /// <summary>
    /// Creates a new instance of <see cref="SqliteTestDatabase"/> and applies database migrations using the specified migrator.
    /// </summary>
    /// <param name="migrator">An implementation of <see cref="IDbMigrator"/> to apply database migrations.</param>
    /// <param name="token">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains an instance of <see cref="SqliteTestDatabase"/> with the migrations applied.</returns>
    public async Task BuildAsync(IDbMigrator migrator, CancellationToken token = default)
    {
        Connection = new DbConnectionInfo(ProviderType.Sqlite, _tempDb);
        await migrator.Migrate(Connection, token);
    }

    /// <summary>
    /// Deletes all data from all tables in the SQLite database, except for system tables
    /// and the 'SchemaVersions' table. This method is executed after each test to ensure
    /// a clean database state for later tests.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation of purging the database.</returns>
    public async Task Purge()
    {
        await using var connection = new SqliteConnection($"Data Source={_tempDb}");

        // Get all tables to delete data from.
        var names = await connection.QueryAsync<string>(
            """
            SELECT name
            FROM sqlite_master
            WHERE type = 'table'
              AND name NOT LIKE 'sqlite%'
              AND name NOT IN ('SchemaVersions', 'target_component', 'operand')
            """
        );

        var deletes = string.Join('\n', names.Select(n => $"DELETE FROM {n} WHERE 1=1;"));

        await connection.ExecuteAsync(
            $"""
             PRAGMA foreign_keys = OFF;
             BEGIN TRANSACTION;
             {deletes}
             COMMIT;
             PRAGMA foreign_keys = ON;
             """);
    }

    /// <summary>
    /// Releases the resources used by the SqliteTestDatabase instance.
    /// Deletes the temporary SQLite database file created for testing purposes.
    /// This method ensures a best-effort cleanup of the database file, even if an exception occurs.
    /// </summary>
    public void Dispose()
    {
        try
        {
            if (File.Exists(_tempDb))
                File.Delete(_tempDb);
        }
        catch
        {
            // Best-effort cleanup
        }
    }
}