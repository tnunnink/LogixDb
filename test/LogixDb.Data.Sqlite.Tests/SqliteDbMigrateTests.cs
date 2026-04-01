using NUnit.Framework.Legacy;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbMigrateTests : SqliteTestFixture
{
    /// <summary>
    /// This test is mostly just to refresh a local db instance to inspect and write queries against.
    /// The base test fixture will create and migrate the database
    /// </summary>
    [Test]
    [Explicit("Only run this locally as it is to refresh a database in the project folder")]
    public async Task MigrateLocalTestDatabaseForWritingQueriesAgainst()
    {
        var connection = new DbConnection(DbProvider.Sqlite, "../../../logix.db");
        var database = new SqliteDb(connection);
        await database.Drop();
        await database.Migrate();
        FileAssert.Exists("../../../logix.db");
    }

    [Test]
    public async Task Migrate_IncludeSpecificTableNames_ShouldOnlyHaveConfiguredTables()
    {
        var options = new DbOptions { Include = ["controller", "tag"] };
        
        await Database.Migrate(options);

        await AssertTableExists("target");
        await AssertTableExists("snapshot");
        await AssertTableExists("operand");
        await AssertTableExists("controller");
        await AssertTableExists("tag");
        await AssertTableDoesNotExists("aoi");
        await AssertTableDoesNotExists("aoi_parameter");
        await AssertTableDoesNotExists("aoi_logic");
        await AssertTableDoesNotExists("data_type");
        await AssertTableDoesNotExists("data_type_member");
        await AssertTableDoesNotExists("task");
        await AssertTableDoesNotExists("program");
        await AssertTableDoesNotExists("routine");
        await AssertTableDoesNotExists("rung");
        await AssertTableDoesNotExists("instruction");
    }
}