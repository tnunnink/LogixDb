using System.Data;
using System.Data.Common;
using LogixDb.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace LogixDb.Sqlite;

/// <summary>
/// Represents a session for interacting with a SQLite database.
/// This class provides access to the database connection and transaction
/// through strongly typed methods, ensuring type safety and consistency
/// when working with the underlying SQLite infrastructure.
/// </summary>
internal class SqliteDatabaseSession(SqliteConnection connection, DbTransaction transaction) : ILogixDatabaseSession
{
    /// <inheritdoc />
    public TConnection GetConnection<TConnection>() where TConnection : IDbConnection
    {
        if (connection is not TConnection typed)
            throw new InvalidOperationException(
                $"The requested connection type '{typeof(TConnection).Name}' is not supported. Expected '{nameof(SqliteConnection)}'.");

        return typed;
    }

    /// <inheritdoc />
    public TTransaction GetTransaction<TTransaction>() where TTransaction : IDbTransaction
    {
        if (transaction is not TTransaction typed)
            throw new InvalidOperationException(
                $"The requested transaction type '{typeof(TTransaction).Name}' is not supported. Expected '{nameof(SqliteTransaction)}'.");

        return typed;
    }
}