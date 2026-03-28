using Dapper;
using Microsoft.Data.Sqlite;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Represents a database session that manages a SQLite connection and transaction as a single unit.
/// Ensures that all database operations within the session are executed within the same connection and transaction context.
/// </summary>
internal sealed class SqliteDbSession : IAsyncDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteDbSession"/> class.
    /// </summary>
    /// <param name="connection">The SQLite connection to use for the session.</param>
    /// <param name="transaction">The transaction to use for the session.</param>
    private SqliteDbSession(SqliteConnection connection, SqliteTransaction transaction)
    {
        Connection = connection;
        Transaction = transaction;
    }

    /// <summary>
    /// The underlying SQLite connection used for the session.
    /// </summary>
    public SqliteConnection Connection { get; }

    /// <summary>
    /// The transaction that encapsulates all operations within the session.
    /// </summary>
    public SqliteTransaction Transaction { get; }

    /// <summary>
    /// Starts a new database session by opening a connection and beginning a transaction.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the SQLite database.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to observe cancellation requests during the session startup.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the initialized <see cref="SqliteDbSession"/>.</returns>
    /// <exception cref="SqliteException">Thrown when the connection cannot be opened or the transaction cannot be started.</exception>
    public static async Task<SqliteDbSession> Start(string connectionString, CancellationToken token)
    {
        var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(token);
        var transaction = (SqliteTransaction)await connection.BeginTransactionAsync(token);
        return new SqliteDbSession(connection, transaction);
    }

    /// <summary>
    /// Executes the specified SQL command asynchronously using the current database session's
    /// connection and transaction context.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="param">The parameters to pass to the SQL command. Can be null.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ExecuteAsync(string sql, object? param = null)
    {
        await Connection.ExecuteAsync(sql, param, Transaction);
    }
    
    /// <summary>
    /// Executes the specified SQL command asynchronously using the current database session's
    /// connection and transaction context.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="param">The parameters to pass to the SQL command. Can be null.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<T> GetAsync<T>(string sql, object? param = null)
    {
        return Connection.QuerySingleAsync<T>(sql, param, Transaction);
    }

    /// <summary>
    /// Executes the specified query and returns all rows as a collection of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the objects to be returned.</typeparam>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="param">The parameters to be passed to the SQL query, if any.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> with all results of the query.</returns>
    public Task<IEnumerable<T>> GetAllAsync<T>(string sql, object? param = null)
    {
        return Connection.QueryAsync<T>(sql, param, Transaction);
    }

    /// <summary>
    /// Commits the transaction, persisting all changes made within the session to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to observe cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous commit operation.</returns>
    public Task Commit(CancellationToken cancellationToken)
    {
        return Transaction.CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Rolls back the transaction, discarding all changes made within the session.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to observe cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous rollback operation.</returns>
    public Task Rollback(CancellationToken cancellationToken)
    {
        return Transaction.RollbackAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously disposes the session, releasing the transaction and connection resources.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await Transaction.DisposeAsync();
        await Connection.DisposeAsync();
    }
}
