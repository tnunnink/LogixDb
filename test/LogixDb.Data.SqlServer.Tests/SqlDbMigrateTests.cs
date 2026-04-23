using Microsoft.Extensions.Logging.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbMigrateTest : SqlServerTestFixture
{
    /// <summary>
    /// This test is mostly just to refresh a local db instance to inspect and write queries against.
    /// The base test fixture will create and migrate the database
    /// </summary>
    [Test]
    [Explicit("Manually run against local test server to check migrations and develop SQL queries against")]
    public async Task MigrateLocalTestDatabaseForWritingQueriesAgainst()
    {
        var connectionInfo = new DbConnectionInfo(
            Provider: DbProvider.SqlServer,
            Source: "localhost,1433",
            Database: "logixdb",
            User: "sa",
            Password: "LogixDb!Test123",
            Trust: true
        );

        var database = new SqlServerManager(connectionInfo, new FakeLogger());
        await database.Drop();
        await database.Migrate();
    }

    [Test]
    public async Task Migrate_OnlyControllerAndTagBasedTables_ShouldOnlyHaveExpectedTables()
    {
        await Database.Migrate(ComponentOptions.Controller | ComponentOptions.Tag);

        // Required Tables
        await AssertTableExists("target");
        await AssertTableExists("target_version");
        await AssertTableExists("target_instance");
        await AssertTableExists("target_info");

        // Included Tables
        await AssertTableExists("controller");
        await AssertTableExists("task");
        await AssertTableExists("program");
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
        await AssertTableDoesNotExists("routine");
        await AssertTableDoesNotExists("rung");
        await AssertTableDoesNotExists("instruction");
        await AssertTableDoesNotExists("argument");
        await AssertTableDoesNotExists("aoi");
        await AssertTableDoesNotExists("aoi_parameter");
        await AssertTableDoesNotExists("aoi_rung");
        await AssertTableDoesNotExists("operand");
    }

    [Test]
    public async Task Migrate_OnlyLogicBasedTables_ShouldOnlyHaveLogicTables()
    {
        await Database.Migrate(ComponentOptions.Logic);

        // Required Tables
        await AssertTableExists("target");
        await AssertTableExists("target_version");
        await AssertTableExists("target_instance");
        await AssertTableExists("target_info");

        // Included Tables
        await AssertTableExists("task");
        await AssertTableExists("program");
        await AssertTableExists("routine");
        await AssertTableExists("rung");
        await AssertTableExists("instruction");
        await AssertTableExists("argument");
        await AssertTableExists("operand");

        // Excluded Tables
        await AssertTableDoesNotExists("controller");
        await AssertTableDoesNotExists("data_type");
        await AssertTableDoesNotExists("data_type_member");
        await AssertTableDoesNotExists("module");
        await AssertTableDoesNotExists("aoi");
        await AssertTableDoesNotExists("aoi_parameter");
        await AssertTableDoesNotExists("aoi_rung");
        await AssertTableDoesNotExists("tag");
        await AssertTableDoesNotExists("tag_member");
        await AssertTableDoesNotExists("tag_comment");
        await AssertTableDoesNotExists("tag_producer");
        await AssertTableDoesNotExists("tag_consumer");
        await AssertTableDoesNotExists("tag_alias");
    }

    [Test]
    public async Task Migrate_NoComponentTables_ShouldHaveNoComponentTables()
    {
        await Database.Migrate(ComponentOptions.None);

        // Required Tables
        await AssertTableExists("target");
        await AssertTableExists("target_version");
        await AssertTableExists("target_instance");
        await AssertTableExists("target_info");

        // Excluded Tables
        await AssertTableDoesNotExists("task");
        await AssertTableDoesNotExists("program");
        await AssertTableDoesNotExists("routine");
        await AssertTableDoesNotExists("rung");
        await AssertTableDoesNotExists("instruction");
        await AssertTableDoesNotExists("argument");
        await AssertTableDoesNotExists("operand");
        await AssertTableDoesNotExists("controller");
        await AssertTableDoesNotExists("data_type");
        await AssertTableDoesNotExists("data_type_member");
        await AssertTableDoesNotExists("module");
        await AssertTableDoesNotExists("aoi");
        await AssertTableDoesNotExists("aoi_parameter");
        await AssertTableDoesNotExists("aoi_rung");
        await AssertTableDoesNotExists("tag");
        await AssertTableDoesNotExists("tag_member");
        await AssertTableDoesNotExists("tag_comment");
        await AssertTableDoesNotExists("tag_producer");
        await AssertTableDoesNotExists("tag_consumer");
        await AssertTableDoesNotExists("tag_alias");
    }
}