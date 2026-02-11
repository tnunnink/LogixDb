using LogixDb.Core.Common;

namespace LogixDb.Core.Abstractions;

/// <summary>
/// Provides factory methods for creating and establishing connections to LogixDB databases.
/// This factory abstracts the creation and initialization of database instances,
/// allowing clients to connect to databases using SQL connection information.
/// </summary>
public interface ILogixDatabaseFactory
{
    /// <summary>
    /// Creates and establishes a connection to a LogixDB database using the specified SQL connection information.
    /// </summary>
    /// <param name="info">The SQL connection information containing provider, server, database, and authentication details.</param>
    /// <returns>An instance of <see cref="ILogixDatabase"/> representing the connected database.</returns>
    ILogixDatabase Connect(SqlConnectionInfo info);
}