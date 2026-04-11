using Dapper;
using Microsoft.Data.SqlClient;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Represents a database session that manages a SQL Server connection and transaction as a single unit.
/// Ensures that all database operations within the session are executed within the same connection and transaction context.
/// </summary>
internal sealed class SqlDbSession : IAsyncDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDbSession"/> class.
    /// </summary>
    /// <param name="connection">The SQL Server connection to use for the session.</param>
    /// <param name="transaction">The transaction to use for the session.</param>
    private SqlDbSession(SqlConnection connection, SqlTransaction transaction)
    {
        Connection = connection;
        Transaction = transaction;
    }

    /// <summary>
    /// The underlying SQL Server connection used for the session.
    /// </summary>
    public SqlConnection Connection { get; }

    /// <summary>
    /// The transaction that encapsulates all operations within the session.
    /// </summary>
    public SqlTransaction Transaction { get; }

    /// <summary>
    /// Starts a new database session by opening a connection and beginning a transaction.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the SQL Server database.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to observe cancellation requests during the session startup.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the initialized <see cref="SqlDbSession"/>.</returns>
    /// <exception cref="SqlException">Thrown when the connection cannot be opened or the transaction cannot be started.</exception>
    public static async Task<SqlDbSession> Start(string connectionString, CancellationToken token)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(token);
        var transaction = (SqlTransaction)await connection.BeginTransactionAsync(token);
        return new SqlDbSession(connection, transaction);
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
    /// Executes a SQL query and returns a single result of the specified type, or the default value if no result is found.
    /// </summary>
    /// <param name="sql">The SQL query to be executed.</param>
    /// <param name="param">The parameters to be used in the query. This can be null if no parameters are required.</param>
    /// <typeparam name="T">The type of the result to be returned.</typeparam>
    /// <returns>A task representing the asynchronous operation, containing the result of the query or the default value if no result is found.</returns>
    public Task<T?> GetOrDefaultAsync<T>(string sql, object? param = null)
    {
        return Connection.QuerySingleOrDefaultAsync<T>(sql, param, Transaction);
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
        await Connection.DisposeAsync();
        await Transaction.DisposeAsync();
    }
}