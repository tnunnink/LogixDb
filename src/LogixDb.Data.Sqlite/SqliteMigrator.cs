using System.Reflection;
using Dapper;
using DbUp;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Sqlite.Scripts.Migrations;
using Microsoft.Data.Sqlite;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// 
/// </summary>
public class SqliteMigrator : IDbMigrator
{
    /// <inheritdoc />
    public async Task<bool> Migrate(DbConnectionInfo connection, CancellationToken token = default)
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

        //todo perhaps have a better result type here
        return result.Successful;
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