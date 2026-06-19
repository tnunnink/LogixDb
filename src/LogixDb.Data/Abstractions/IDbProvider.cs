using System.Data.Common;

namespace LogixDb.Data.Abstractions;

/// <summary>
/// Provides an abstraction for database operations including connection management,
/// data writing, and script retrieval. Implementations of this interface are responsible
/// for managing database-specific functionality and resources.
/// </summary>
public interface IDbProvider
{
    /// <summary>
    /// Opens and returns a new database connection asynchronously.
    /// </summary>
    /// <param name="token">The <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Task{DbConnection}"/> that represents the asynchronous operation and contains the opened database connection.</returns>
    Task<DbConnection> OpenConnection(CancellationToken token = default);

    /// <summary>
    /// Gets a database writer instance for performing bulk write operations to the database.
    /// </summary>
    /// <returns>An <see cref="IDbWriter"/> instance for writing data to the database.</returns>
    IDbWriter GetWriter();

    /// <summary>
    /// Retrieves the SQL script associated with the specified <see cref="ScriptName"/>.
    /// </summary>
    /// <param name="scriptName">The <see cref="ScriptName"/> enumeration value representing the desired script.</param>
    /// <returns>A <see cref="string"/> containing the SQL script corresponding to the provided script name.</returns>
    string GetScript(ScriptName scriptName);

    /// <summary>
    /// Retrieves the merge script for the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table for which the merge script is requested.</param>
    /// <returns>A string representing the merge script for the given table.</returns>
    string GetMergeScript(string tableName);
}