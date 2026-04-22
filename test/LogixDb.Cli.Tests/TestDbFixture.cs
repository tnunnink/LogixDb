using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogixDb.Cli.Tests;

/// <summary>
/// Abstract base class that provides a temporary SQLite database fixture for use in unit tests.
/// </summary>
/// <remarks>
/// This class creates a unique SQLite database file for each test execution and provides access to
/// a database manager instance via the <see cref="Database"/> property. The database file is automatically
/// cleaned up after the tests are run by invoking the <see cref="TearDown"/> method.
/// </remarks>
public abstract class TestDbFixture
{
    protected TestDbFixture()
    {
        DbConnection = Path.Combine(Path.GetTempPath(), $"LogixTest_{Guid.NewGuid():N}.db");
        var connectionInfo = new DbConnectionInfo(DbProvider.Sqlite, DbConnection);
        Database = new SqliteManager(connectionInfo, NullLogger.Instance);
    }

    /// <summary>
    /// Gets the file path of the temporary database used during tests.
    /// This property generates a unique file path within the system's temporary directory
    /// and is used to create a testing SQLite database instance. The database file
    /// is cleaned up after the tests using the <see cref="TearDown"/> method to ensure proper disposal.
    /// </summary>
    protected string DbConnection { get; }

    /// <summary>
    /// Provides access to the database instance used during testing.
    /// This property is an implementation of the <see cref="IDbManager"/> interface and is
    /// initialized with a SQLite database connection. The database is bound to the temporary
    /// file path defined by the <c>TempDb</c> property, ensuring a unique and isolated testing context.
    /// </summary>
    protected IDbManager Database { get; }

    /// <summary>
    /// Cleans up the temporary database created during tests by removing the
    /// associated file from the file system. This method ensures that no
    /// residual test data is left behind after the test execution.
    /// </summary>
    /// <remarks>
    /// The method performs a best-effort attempt to delete the file at the path
    /// specified by the <see cref="DbConnection"/> property. If the file cannot be
    /// deleted due to any exceptions, those are silently ignored.
    /// </remarks>
    [TearDown]
    protected void TearDown()
    {
        try
        {
            if (File.Exists(DbConnection))
            {
                File.Delete(DbConnection);
            }
        }
        catch (Exception)
        {
            // best effort
        }
    }
}