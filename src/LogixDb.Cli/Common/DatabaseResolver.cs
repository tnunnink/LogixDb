using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Sqlite;
using LogixDb.Data.SqlServer;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogixDb.Cli.Common;

/// <summary>
/// Provides static methods to create and manage database connections using different SQL providers.
/// </summary>
public static class DatabaseResolver
{
    /// <summary>
    /// Creates an instance of a database connection using the specified SQL provider and connection information.
    /// </summary>
    /// <param name="connection">The connection information that includes the provider type, data source, and optional credentials.</param>
    /// <returns>An instance of <see cref="IDbManager"/> corresponding to the specified SQL provider.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided SQL provider is not supported.</exception>
    public static IDbManager GetDatabase(DbConnectionInfo connection)
    {
        return connection.Provider switch
        {
            DbProvider.Sqlite => new SqliteManager(connection, NullLogger.Instance),
            DbProvider.SqlServer => new SqlServerManager(connection, NullLogger.Instance),
            _ => throw new ArgumentOutOfRangeException(nameof(connection), connection.Provider,
                "Unsupported SQL provider")
        };
    }
}