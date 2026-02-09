using System.Data;
using FluentMigrator.Runner;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace LogixDb.Sqlite;

/// <summary>
/// 
/// </summary>
/// <param name="dataSource"></param>
public class LogixSqliteDatabase(string dataSource)
{
    private readonly string _connectionString = BuildConnectionString(dataSource);

    public void Migrate()
    {
        using var provider = BuildServiceProvider(_connectionString);
        using var scope = provider.CreateScope();
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    /// <summary>
    /// Establishes and opens an asynchronous connection to the SQLite database using the configured connection string.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an open database connection.</returns>
    private async Task<IDbConnection> Connect(CancellationToken token)
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
                .ScanIn(typeof(LogixSqliteDatabase).Assembly).For.Migrations());

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Builds a SQLite connection string with the specified data source and configured options.
    /// </summary>
    /// <param name="dataSource">The path to the SQLite database file.</param>
    /// <returns>A formatted SQLite connection string with foreign keys enabled and pooling disabled.</returns>
    private static string BuildConnectionString(string dataSource)
    {
        if (string.IsNullOrEmpty(dataSource))
            throw new ArgumentException("The dataSource parameter cannot be null or empty.", nameof(dataSource));

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = dataSource,
            ForeignKeys = true,
            Pooling = false
        };

        return builder.ConnectionString;
    }
}