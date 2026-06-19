using System.ComponentModel.DataAnnotations;
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Sqlite;

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
    /// 
    /// </summary>
    /// <param name="console"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask ExecuteAsync(IConsole console, CancellationToken token);

    /// <summary>
    /// Parses the database connection string into a <see cref="DbConnectionInfo"/> object.
    /// This method uses the <see cref="DbConnectionInfo.Parse(string)"/> method to interpret
    /// the provided connection string and extract details about the database provider, source,
    /// authentication credentials, and other related settings.
    /// </summary>
    /// <returns>
    /// A <see cref="DbConnectionInfo"/> instance containing the parsed details of the
    /// database connection string.
    /// </returns>
    protected DbConnectionInfo ParseConnection() => DbConnectionInfo.Parse(Connection);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="Exception"></exception>
    protected static IDbMigrator GetMigrator(DbConnectionInfo connection)
    {
        return connection.Provider switch
        {
            DbProvider.Sqlite => new SqliteMigrator(),
            DbProvider.SqlServer => throw new NotImplementedException(),
            _ => throw new Exception($"Unsupported SQL provider: {connection.Provider}")
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="Exception"></exception>
    protected static IDbManager GetManager(DbConnectionInfo connection)
    {
        var provider = connection.Provider switch
        {
            DbProvider.Sqlite => new SqliteProvider(connection),
            DbProvider.SqlServer => throw new NotImplementedException(),
            _ => throw new Exception($"Unsupported SQL provider: {connection.Provider}")
        };

        return new DbManager(provider);
    }
}