using LogixDb.Data.SqlServer;
using LogixDb.Testing.SqlServer;

namespace LogixDb.Service.Tests;

/// <summary>
/// Provides an SQL Server test environment for use in integration tests.
/// This class is responsible for setting up and tearing down a transient SQL Server database,
/// ensuring a clean and isolated state for each testing session.
/// </summary>
[SetUpFixture]
public static class SqlServerEnvironment
{
    /// <summary>
    /// Represents the SQL Server test database instance used for testing purposes.
    /// This static property provides access to an instance of <c>SqlServerTestDatabase</c>,
    /// which facilitates testing by managing a transient SQL Server database environment.
    /// The database is initialized at the start of the test suite and properly destroyed
    /// after the tests are completed to ensure resource cleanup.
    /// </summary>
    public static readonly SqlServerTestDatabase Database = new();

    [OneTimeSetUp]
    public static Task Setup() => Database.StartAsync(new SqlServerMigrator());

    [OneTimeTearDown]
    public static Task Teardown() => Database.StopAsync();
}