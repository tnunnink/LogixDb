using System.ComponentModel.DataAnnotations;
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Sqlite;
using LogixDb.Data.SqlServer;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents the base class for CLI commands that interact with a database.
/// This class handles common database-related operations such as configuring
/// connection information and executing commands asynchronously.
/// </summary>
[PublicAPI]
public abstract class DbCommand : ICommand
{
    [Required]
    [CommandOption("connection", 'c', Description = "The database connection string.")]
    public string Connection { get; set; } = string.Empty;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(IConsole console)
    {
        if (string.IsNullOrWhiteSpace(Connection))
            throw new CommandException("Database argument 'connection' is required.", ErrorCodes.UsageError);

        var cancellation = console.RegisterCancellationHandler();
        return ExecuteAsync(console, cancellation);
    }

    /// <summary>
    /// When overridden in a derived class, executes the database command asynchronously
    /// with the specified console interface and cancellation token.
    /// </summary>
    /// <param name="console">The console instance used for input/output operations and rendering.</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    protected abstract ValueTask ExecuteAsync(IConsole console, CancellationToken token);

    /// <summary>
    /// Creates and returns an appropriate database migrator instance based on the
    /// database provider specified in the connection string.
    /// </summary>
    /// <returns>
    /// An <see cref="IDbMigrator"/> instance corresponding to the database provider:
    /// <see cref="SqliteMigrator"/> for SQLite databases, or
    /// <see cref="SqlServerMigrator"/> for SQL Server databases.
    /// </returns>
    /// <exception cref="CommandException">
    /// Thrown when the database provider specified in the connection string is not supported.
    /// </exception>
    protected IDbMigrator GetMigrator()
    {
        var connection = DbConnectionInfo.Parse(Connection);
        return connection.ProviderType switch
        {
            ProviderType.Sqlite => new SqliteMigrator(),
            ProviderType.SqlServer => new SqlServerMigrator(),
            _ => throw new CommandException($"Unsupported SQL provider: {connection.ProviderType}")
        };
    }

    /// <summary>
    /// Creates and returns an appropriate database manager instance based on the
    /// database provider specified in the connection string.
    /// </summary>
    /// <returns>
    /// An <see cref="IDbManager"/> instance that manages database operations
    /// using the appropriate provider (<see cref="SqliteProvider"/> for SQLite
    /// databases or <see cref="SqlServerProvider"/> for SQL Server databases).
    /// </returns>
    /// <exception cref="CommandException">
    /// Thrown when the database provider specified in the connection string is not supported.
    /// </exception>
    protected IDbManager GetManager()
    {
        var connection = DbConnectionInfo.Parse(Connection);

        IDbProvider provider = connection.ProviderType switch
        {
            ProviderType.Sqlite => new SqliteProvider(connection),
            ProviderType.SqlServer => new SqlServerProvider(connection),
            _ => throw new CommandException($"Unsupported SQL provider: {connection.ProviderType}")
        };

        return new DbManager(provider);
    }
}