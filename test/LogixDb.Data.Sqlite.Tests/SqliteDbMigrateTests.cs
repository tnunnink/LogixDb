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
        var connection = new DbConnectionInfo(DbProvider.Sqlite, "../../../logix.db");
        var database = new SqliteDb(connection);
        await database.Drop();
        await database.Migrate();
        FileAssert.Exists("../../../logix.db");
    }

    [Test]
    public async Task Migrate_IncludeSpecificTableNames_ShouldOnlyHaveConfiguredTables()
    {
        var options = new TableOptions { Include = ["controller", "tag"] };
        
        await Database.Migrate(options);

        // Required Tables
        await AssertTableExists("target");
        await AssertTableExists("snapshot");
        await AssertTableExists("snapshot_property");
        await AssertTableExists("operand");
        
        // Included Tables
        await AssertTableExists("controller");
        await AssertTableExists("tag");
        await AssertTableExists("tag_member");
        await AssertTableExists("tag_comment");
        await AssertTableExists("tag_producer");
        await AssertTableExists("tag_consumer");
        await AssertTableExists("tag_alias");
        
        // Excluded Tables
        await AssertTableDoesNotExists("data_type");
        await AssertTableDoesNotExists("data_type_member");
        await AssertTableDoesNotExists("module");
        await AssertTableDoesNotExists("task");
        await AssertTableDoesNotExists("program");
        await AssertTableDoesNotExists("routine");
        await AssertTableDoesNotExists("rung");
        await AssertTableDoesNotExists("instruction");
        await AssertTableDoesNotExists("argument");
        await AssertTableDoesNotExists("aoi");
        await AssertTableDoesNotExists("aoi_parameter");
        await AssertTableDoesNotExists("aoi_rung");
    }

    [Test]
    public async Task Migrate_ExcludeSpecificTableNames_ShouldNotHaveExcludedTables()
    {
        var options = new TableOptions { Exclude = ["instruction", "rung"] };

        await Database.Migrate(options);

        // Required Tables
        await AssertTableExists("target");
        await AssertTableExists("snapshot");
        await AssertTableExists("snapshot_property");
        await AssertTableExists("operand");

        // Included Tables (not explicitly excluded, and Include is empty)
        await AssertTableExists("controller");
        await AssertTableExists("data_type");
        await AssertTableExists("data_type_member");
        await AssertTableExists("module");
        await AssertTableExists("task");
        await AssertTableExists("program");
        await AssertTableExists("routine");
        await AssertTableExists("aoi");
        await AssertTableExists("aoi_parameter");
        await AssertTableExists("aoi_rung");
        await AssertTableExists("tag");
        await AssertTableExists("tag_member");
        await AssertTableExists("tag_comment");
        await AssertTableExists("tag_producer");
        await AssertTableExists("tag_consumer");
        await AssertTableExists("tag_alias");
        
        // Excluded Tables
        // Since "rung" tag is excluded, all tables with "rung" tag should be missing
        // "rung", "instruction", "argument" all have MigrationTag.Rung
        await AssertTableDoesNotExists("rung");
        await AssertTableDoesNotExists("instruction");
        await AssertTableDoesNotExists("argument");
    }
}