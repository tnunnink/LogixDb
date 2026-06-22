using Dapper;
using DotNet.Testcontainers.Containers;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace LogixDb.Testing.SqlServer;

/// <summary>
/// Provides a test infrastructure for SQL Server by managing a test container and enabling database operations.
/// This class facilitates the creation, teardown, and purging of a transient SQL Server database for testing purposes.
/// </summary>
public class SqlServerTestDatabase
{
    /// <summary>
    /// Represents a test container instance for managing a Microsoft SQL Server database lifecycle
    /// within the SqlServerTestFixture. This container facilitates integration testing by providing
    /// an isolated and disposable SQL Server environment.
    /// </summary>
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("LogixDb!Test123")
        .Build();

    /// <summary>
    /// Represents the database connection information required to establish a connection
    /// to a Microsoft SQL Server instance. This includes details such as server address,
    /// database name, authentication credentials, and provider type, used mainly for
    /// configuring and managing the database lifecycle within the testing fixture.
    /// </summary>
    public DbConnectionInfo Connection { get; private set; } = null!;

    /// <summary>
    /// Initializes and starts the test SQL Server database container and performs necessary database
    /// migrations using the provided migrator. This method ensures that the test database is ready to use
    /// with updated schemas and configurations.
    /// </summary>
    /// <param name="migrator">The database migrator responsible for executing schema updates on the test database during initialization.</param>
    /// <returns>A task representing the asynchronous operation of starting the container and applying migrations.</returns>
    public async Task StartAsync(IDbMigrator migrator)
    {
        await _container.StartAsync();

        Connection = new DbConnectionInfo(
            DbProvider.SqlServer,
            _container.Hostname,
            Database: "logixdb",
            Port: _container.GetMappedPublicPort(),
            User: "sa",
            Password: "LogixDb!Test123"
        );

        await migrator.Migrate(Connection);
    }

    /// <summary>
    /// Stops the test SQL Server database container if it is currently running and disposes of its resources.
    /// This method ensures proper cleanup of the container and releases associated resources for the test environment.
    /// </summary>
    /// <returns>A task representing the asynchronous operation of stopping and disposing of the container.</returns>
    public async Task StopAsync()
    {
        if (_container.State == TestcontainersStates.Running)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }

    /// <summary>
    /// Cleans up the database by removing all test-related data while leaving specific seed or lookup
    /// tables intact. This method disables constraints, deletes records in the specified schema, and
    /// re-enables constraints to ensure the database is in a consistent state for later tests.
    /// This approach is optimized to avoid dropping the database, making the cleanup faster.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PurgeAsync()
    {
        // Get the container connection details but update to the logixdb database to query tables.
        var builder = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "logixdb"
        };

        await using var connection = new SqlConnection(builder.ConnectionString);

        // Get all tables to delete data from.
        var names = await connection.QueryAsync<string>(
            """
            SELECT QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME)
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = 'logix' 
              AND TABLE_NAME NOT IN ('target_component', 'operand')
            """
        );

        var statements = string.Join('\n', names.Select(n => $"DELETE FROM {n};"));

        await connection.ExecuteAsync(
            $"""
             -- Disable all constraints for the session
             EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'

             BEGIN TRANSACTION;
             {statements}
             COMMIT TRANSACTION;

             -- Re-enable all constraints
             EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'
             """);
    }
}