using System.Reflection;
using Dapper;
using DbUp;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Sqlite.Scripts.Migrations;
using Microsoft.Data.Sqlite;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides functionality for applying database migrations to a SQLite database.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IDbMigrator"/> interface, enabling support for
/// running migrations in SQLite-based systems. It uses embedded migration scripts defined
/// within the assembly and performs versioning and upgrade operations.
/// </remarks>
public class SqliteMigrator : IDbMigrator
{
    /// <inheritdoc />
    public async Task<MigrationResult> Migrate(DbConnectionInfo connection, CancellationToken token = default)
    {
        var connectionString = connection.ToConnectionString();
        await ConfigureDatabase(connectionString, token);

        var upgrader = DeployChanges.To
            .SqliteDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.Contains("Migrations"))
            .WithScript(nameof(Logix_202606180900_SeedOperands), new Logix_202606180900_SeedOperands())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        // Aggregate the error message using the failed script name and exception message.
        var error = result.Error is not null
            ? $"Failed to execute migration '{result.ErrorScript?.Name}' with exception: {result.Error}"
            : null;

        return new MigrationResult
        {
            Success = result.Successful,
            Error = error,
            Executed = result.Scripts.Select(s => s.Name).ToList()
        };
    }

    /// <summary>
    /// Configures SQLite performance-related PRAGMA settings to enhance database operations.
    /// This method is responsible for setting various SQLite pragmas, such as journal mode,
    /// auto vacuum mode, synchronization settings, and memory usage configurations, 
    /// to optimize the database performance.
    /// </summary>
    /// <remarks>
    /// The method adjusts settings including WAL (Write-Ahead Logging) for journaling,
    /// incremental vacuum, and memory configurations such as mmap size and cache size.
    /// These settings are tailored to improve performance while maintaining data integrity.
    /// </remarks>
    private static async Task ConfigureDatabase(string connectionString, CancellationToken token)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(token);

        await connection.ExecuteAsync(
            """
            PRAGMA journal_mode = WAL;
            PRAGMA auto_vacuum = FULL;
            PRAGMA temp_store = MEMORY;
            PRAGMA busy_timeout = 5000;
            PRAGMA page_size = 16384;
            """
        );
    }
}