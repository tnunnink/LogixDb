using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Sqlite;
using LogixDb.Data.SqlServer;
using Microsoft.Extensions.Logging;

namespace LogixDb.Cli.Common;

/// <summary>
/// Provides functionality for managing and resolving database managers and repositories
/// based on specified connection information.
/// </summary>
public class DatabaseProvider(ILogger logger) : IDbProvider
{
    /// <inheritdoc />
    public IDbManager Resolve(DbConnectionInfo connection)
    {
        return connection.Provider switch
        {
            DbProvider.Sqlite => new SqliteManager(connection, logger),
            DbProvider.SqlServer => new SqlServerManager(connection, logger),
            _ => throw new ArgumentOutOfRangeException(nameof(connection), connection.Provider,
                "Unsupported SQL provider")
        };
    }
}