using System.Reflection;
using DbUp;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Sqlite.Scripts.Migrations;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Provides the implementation of <see cref="IDbMigrator"/> for performing
/// database migrations on a Microsoft SQL Server database.
/// </summary>
/// <remarks>
/// This class uses the DbUp library to apply migrations from embedded scripts
/// in the assembly. It aggregates migration results, including success status,
/// errors, and executed scripts.
/// </remarks>
public class SqlServerMigrator : IDbMigrator
{
    /// <inheritdoc />
    public Task<MigrationResult> Migrate(DbConnectionInfo connection, CancellationToken token = default)
    {
        var connectionString = connection.ToConnectionString();

        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.Contains("Migrations"))
            .WithScript(nameof(Logix_202606180900_SeedOperands), new Logix_202606180900_SeedOperands())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        // Aggregate the error message using the failed script name and exception message.
        var error = result.Error is not null
            ? $"Failed to execute migration '{result.ErrorScript?.Name}' with exception: {result.Error}"
            : null;

        return Task.FromResult(new MigrationResult
        {
            Success = result.Successful,
            Error = error,
            Executed = result.Scripts.Select(s => s.Name).ToList()
        });
    }
}