using System.Data;
using FluentMigrator.Runner;
using LogixDb.Core.Abstractions;
using LogixDb.Core.Common;
using LogixDb.Migrations;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace LogixDb.Sqlite;

/// <summary>
/// 
/// </summary>
/// <param name="info"></param>
public class LogixSqliteDatabase(SqlConnectionInfo info) : ILogixDatabase
{
    private readonly string _connectionString = BuildConnectionString(info);

    public void Migrate()
    {
        using var provider = BuildServiceProvider(_connectionString);
        using var scope = provider.CreateScope();
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    public Task<IEnumerable<Snapshot>> Snapshots(string? targetKey = null, CancellationToken token = default)
    {
        // todo query to table to return the snapshot records optionally filtering by the target.
        throw new NotImplementedException();
    }

    public async Task<Snapshot> Import(Snapshot snapshot, string? targetKey = null, CancellationToken token = default)
    {
        using var connection = await OpenConnection(token);
        using var transaction = connection.BeginTransaction();

        throw new NotImplementedException();
    }

    public Task<Snapshot> Export(string targetKey, CancellationToken token = default)
    {
        // todo get the latest snapshot for the target and decompress and 
        throw new NotImplementedException();
    }

    public Task Purge(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task Purge(string targetKey, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Establishes and opens an asynchronous connection to the SQLite database using the configured connection string.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an open database connection.</returns>
    private async Task<IDbConnection> OpenConnection(CancellationToken token)
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(token);
        return connection;
    }

    /// <summary>
    /// Builds and configures a service provider with FluentMigrator services for database migrations.
    /// </summary>
    /// <param name="connectionString">The connection string to use for database migrations.</param>
    /// <returns>A configured <see cref="ServiceProvider"/> instance with FluentMigrator services registered.</returns>
    private static ServiceProvider BuildServiceProvider(string connectionString)
    {
        var services = new ServiceCollection();

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(
                    typeof(MigrationTableMetaData).Assembly,
                    typeof(LogixSqliteDatabase).Assembly)
                .For.Migrations()
            );

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Constructs a SQLite connection string based on the provided connection information.
    /// </summary>
    /// <param name="info">An instance of <see cref="SqlConnectionInfo"/> containing the necessary details to construct the connection string.</param>
    /// <returns>A string representing the SQLite connection string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="info"/> is null.</exception>
    private static string BuildConnectionString(SqlConnectionInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = info.DataSource,
            ForeignKeys = true,
            Pooling = false
        };

        return builder.ConnectionString;
    }
}