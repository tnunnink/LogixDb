namespace LogixDb.Data.Abstractions;

/// <summary>
/// Defines a contract for database migration operations.
/// </summary>
public interface IDbMigrator
{
    /// <summary>
    /// Applies database migrations to the specified database connection.
    /// </summary>
    /// <param name="connection">The database connection information containing connection string and settings.</param>
    /// <param name="token">A cancellation token to observe while waiting for the migration to complete.</param>
    /// <returns>A task that represents the asynchronous migration operation. The task result contains <c>true</c> if the migration succeeded; otherwise, <c>false</c>.</returns>
    Task<MigrationResult> Migrate(DbConnectionInfo connection, CancellationToken token = default);
}