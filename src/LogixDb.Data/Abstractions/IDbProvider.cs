namespace LogixDb.Data.Abstractions;

/// <summary>
/// Provides an abstraction for resolving instances of <see cref="IDbManager"/>
/// based on the specified database connection information.
/// </summary>
public interface IDbProvider
{
    /// <summary>
    /// Resolves an instance of <see cref="IDbManager"/> based on the provided
    /// database connection information.
    /// </summary>
    /// <param name="connection">
    /// An instance of <see cref="DbConnectionInfo"/> containing the necessary
    /// details to establish a database connection, such as provider type, source,
    /// optional database name, credentials, port, and encryption settings.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IDbManager"/> that can be used to perform
    /// database-related operations, such as migrations or connections.
    /// </returns>
    IDbManager Resolve(DbConnectionInfo connection);
}