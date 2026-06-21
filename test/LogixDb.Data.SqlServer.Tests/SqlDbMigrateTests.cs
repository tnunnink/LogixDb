namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbMigrateTest
{
    /// <summary>
    /// This test is mostly just to refresh a local db instance to inspect and write queries against.
    /// The base test fixture will create and migrate the database
    /// </summary>
    [Test]
    [Explicit("Manually run against local test server to check migrations and develop SQL queries against")]
    public async Task MigrateLocalTestDatabaseForWritingQueriesAgainst()
    {
        var migrator = new SqlServerMigrator();
        
        var connectionInfo = new DbConnectionInfo(
            Provider: DbProvider.SqlServer,
            Source: "localhost,1433",
            Database: "logixdb",
            User: "sa",
            Password: "LogixDb!Test123",
            Trust: true
        );

        var result = await migrator.Migrate(connectionInfo);

        Assert.That(result.Success, Is.True);
    }
}