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
        var database = new SqliteManager(connection);
        await database.Drop();
        await database.Migrate();
        FileAssert.Exists("../../../logix.db");
    }

    [Test]
    public async Task Migrate_OnlyControllerAndTagBasedTables_ShouldOnlyHaveExpectedTables()
    {
        await Database.Migrate(ComponentOptions.Controller | ComponentOptions.Tag);

        // Required Tables
        await AssertTableExists("target");
        await AssertTableExists("target_version");
        await AssertTableExists("target_version_map");
        await AssertTableExists("target_info");

        // Included Tables
        await AssertTableExists("controller");
        await AssertTableExists("task");
        await AssertTableExists("program");
        await AssertTableExists("tag");
        await AssertTableExists("tag_member");
        await AssertTableExists("tag_value");
        await AssertTableExists("tag_comment");
        await AssertTableExists("tag_producer");
        await AssertTableExists("tag_consumer");

        // Excluded Tables
        await AssertTableDoesNotExists("data_type");
        await AssertTableDoesNotExists("data_type_member");
        await AssertTableDoesNotExists("module");
        await AssertTableDoesNotExists("routine");
        await AssertTableDoesNotExists("rung");
        await AssertTableDoesNotExists("rung_instruction");
        await AssertTableDoesNotExists("rung_argument");
        await AssertTableDoesNotExists("rung_reference");
        await AssertTableDoesNotExists("aoi");
        await AssertTableDoesNotExists("aoi_parameter");
        await AssertTableDoesNotExists("operand");
    }

    [Test]
    public async Task Migrate_OnlyLogicBasedTables_ShouldOnlyHaveLogicTables()
    {
        await Database.Migrate(ComponentOptions.Logic);

        // Required Tables
        await AssertTableExists("target");
        await AssertTableExists("target_version");
        await AssertTableExists("target_version_map");
        await AssertTableExists("target_info");

        // Included Tables
        await AssertTableExists("task");
        await AssertTableExists("program");
        await AssertTableExists("routine");
        await AssertTableExists("rung");
        await AssertTableExists("rung_instruction");
        await AssertTableExists("rung_argument");
        await AssertTableExists("rung_reference");
        await AssertTableExists("operand");

        // Excluded Tables
        await AssertTableDoesNotExists("controller");
        await AssertTableDoesNotExists("data_type");
        await AssertTableDoesNotExists("data_type_member");
        await AssertTableDoesNotExists("module");
        await AssertTableDoesNotExists("aoi");
        await AssertTableDoesNotExists("aoi_parameter");
        await AssertTableDoesNotExists("tag");
        await AssertTableDoesNotExists("tag_member");
        await AssertTableDoesNotExists("tag_comment");
        await AssertTableDoesNotExists("tag_producer");
        await AssertTableDoesNotExists("tag_consumer");
        await AssertTableDoesNotExists("tag_value");
    }

    [Test]
    public async Task Migrate_NoComponentTables_ShouldHaveNoComponentTables()
    {
        await Database.Migrate(ComponentOptions.None);

        // Required Tables
        await AssertTableExists("target");
        await AssertTableExists("target_version");
        await AssertTableExists("target_version_map");
        await AssertTableExists("target_info");

        // Excluded Tables
        await AssertTableDoesNotExists("task");
        await AssertTableDoesNotExists("program");
        await AssertTableDoesNotExists("routine");
        await AssertTableDoesNotExists("rung");
        await AssertTableDoesNotExists("rung_instruction");
        await AssertTableDoesNotExists("rung_argument");
        await AssertTableDoesNotExists("rung_reference");
        await AssertTableDoesNotExists("operand");
        await AssertTableDoesNotExists("controller");
        await AssertTableDoesNotExists("data_type");
        await AssertTableDoesNotExists("data_type_member");
        await AssertTableDoesNotExists("module");
        await AssertTableDoesNotExists("aoi");
        await AssertTableDoesNotExists("aoi_parameter");
        await AssertTableDoesNotExists("tag");
        await AssertTableDoesNotExists("tag_member");
        await AssertTableDoesNotExists("tag_comment");
        await AssertTableDoesNotExists("tag_producer");
        await AssertTableDoesNotExists("tag_consumer");
        await AssertTableDoesNotExists("tag_value");
    }
}