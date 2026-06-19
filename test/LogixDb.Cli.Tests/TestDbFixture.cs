using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Sqlite;

namespace LogixDb.Cli.Tests;

/// <summary>
/// Abstract base class that provides a temporary SQLite database fixture for use in unit tests.
/// </summary>
/// <remarks>
/// This class creates a unique SQLite database file for each test execution and provides access to
/// a database manager instance via the <see cref="Manager"/> property. The database file is automatically
/// cleaned up after the tests are run by invoking the <see cref="TearDown"/> method.
/// </remarks>
public abstract class TestDbFixture
{
    protected TestDbFixture()
    {
        Connection = new DbConnectionInfo(
            DbProvider.Sqlite,
            Path.Combine(Path.GetTempPath(), $"LogixTest_{Guid.NewGuid():N}.db")
        );

        Migrator = new SqliteMigrator();
        Manager = new DbManager(new SqliteProvider(Connection));
    }

    /// <summary>
    /// Provides the database connection information used for managing
    /// the temporary SQLite database in unit tests. This property
    /// encapsulates details about the database provider and file path,
    /// enabling configuration and connectivity during test execution.
    /// </summary>
    protected DbConnectionInfo Connection { get; }

    /// <summary>
    /// Provides access to the database migration operations used by the test.
    /// This property is initialized with an instance of <see cref="SqliteMigrator"/>
    /// to handle migrations for the SQLite database created during the test setup.
    /// </summary>
    protected IDbMigrator Migrator { get; }

    /// <summary>
    /// Provides access to the database instance used during testing.
    /// This property is an implementation of the <see cref="IDbManager"/> interface and is
    /// initialized with a SQLite database connection. The database is bound to the temporary
    /// file path defined by the <c>TempDb</c> property, ensuring a unique and isolated testing context.
    /// </summary>
    protected IDbManager Manager { get; }

    /// <summary>
    /// Cleans up the temporary database created during tests by removing the
    /// associated file from the file system. This method ensures that no
    /// residual test data is left behind after the test execution.
    /// </summary>
    [TearDown]
    protected void TearDown()
    {
        try
        {
            if (File.Exists(Connection.Source))
            {
                File.Delete(Connection.Source);
            }
        }
        catch (Exception)
        {
            // best effort
        }
    }
}