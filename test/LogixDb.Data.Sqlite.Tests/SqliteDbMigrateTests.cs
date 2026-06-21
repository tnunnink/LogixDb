using NUnit.Framework.Legacy;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbMigrateTests
{
    /// <summary>
    /// This test is mostly just to refresh a local db instance to inspect and write queries against.
    /// The base test fixture will create and migrate the database
    /// </summary>
    [Test]
    [Explicit("Only run this locally to refresh a database in the project folder")]
    public async Task MigrateLocalTestDatabaseForWritingQueriesAgainst()
    {
        var migrator = new SqliteMigrator();
        
        var connection = new DbConnectionInfo(DbProvider.Sqlite, "../../../logix.db");

        if (File.Exists(connection.Source))
            File.Delete(connection.Source);

        var result = await migrator.Migrate(connection);

        Assert.That(result.Success, Is.True);
        FileAssert.Exists("../../../logix.db");
    }
}