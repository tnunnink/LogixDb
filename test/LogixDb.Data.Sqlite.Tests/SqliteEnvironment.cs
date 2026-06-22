using LogixDb.Testing.Sqlite;

namespace LogixDb.Data.Sqlite.Tests;

/// <summary>
/// Configures and manages a test environment for SQLite databases.
/// Used in integration testing to ensure a consistent database setup and teardown process.
/// </summary>
[SetUpFixture]
public static class SqliteEnvironment
{
    /// <summary>
    /// Gets an instance of the test SQLite database used for integration testing purposes.
    /// This property initializes and provides access to a temporary SQLite database that
    /// facilitates testing operations. The database instance is built during the setup
    /// phase by invoking the <c>BuildAsync</c> method from <see cref="SqliteTestDatabase"/>.
    /// </summary>
    public static SqliteTestDatabase Database { get; } = new();

    [OneTimeSetUp]
    public static Task Setup() => Database.BuildAsync(new SqliteMigrator());

    [OneTimeTearDown]
    public static void Teardown() => Database.Dispose();
}