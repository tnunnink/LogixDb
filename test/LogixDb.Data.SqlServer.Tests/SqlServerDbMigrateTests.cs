namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlServerMigrationTest : SqlServerTestFixture
{
    [Test]
    [Explicit("Manually run against local test server to check migrations and develop SQL queries against")]
    public async Task Build_WithLocalContainerServer_ShouldCreateAndMigrateDatabase()
    {
        var connectionInfo = new DbConnection(
            Provider: DbProvider.SqlServer,
            Source: "localhost,1433",
            Database: "logixdb",
            User: "sa",
            Password: "LogixDb!Test123",
            Trust: true
        );

        var database = new SqlServerDb(connectionInfo);

        await database.Migrate();
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

    [Test]
    public async Task Migrate_ExcludeSpecificTableNames_ShouldNotHaveExcludedTables()
    {
        var options = new DbOptions { Exclude = ["instruction", "rung"] };

        await Database.Migrate(options);

        await AssertTableExists("controller");
        await AssertTableExists("tag");
        await AssertTableDoesNotExists("instruction");
        await AssertTableDoesNotExists("rung");
    }
}