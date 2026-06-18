namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260616Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202606160820_CreatesTypeTreeAtVersionFunction()
    {
        await Database.Migrate(202606160820);

        await AssertFunctionExists("type_tree_at_version");
    }

    [Test]
    public async Task MigrateUp_ToM202606161358_CreatesGetLatestVersionIdFunction()
    {
        await Database.Migrate(202606161358);

        await AssertFunctionExists("get_latest_version_id");
    }

    [Test]
    public async Task MigrateUp_ToM202606162100_CreatesTraceabilityTablesWithExpectedColumns()
    {
        await Database.Migrate(202606162100, ComponentOptions.All);

        using (Assert.EnterMultipleScope())
        {
            // Assert import table
            await AssertTableExists("import");
            await AssertColumnDefinition("import", "import_id", "uniqueidentifier");
            await AssertColumnDefinition("import", "import_status", "nvarchar");
            await AssertColumnDefinition("import", "source_type", "nvarchar");
            await AssertColumnDefinition("import", "file_type", "nvarchar");
            await AssertColumnDefinition("import", "file_name", "nvarchar");
            await AssertColumnDefinition("import", "posted_on", "datetime");
            await AssertPrimaryKey("import", "import_id");

            // Assert import_log table
            await AssertTableExists("import_log");
            await AssertColumnDefinition("import_log", "log_id", "bigint");
            await AssertColumnDefinition("import_log", "import_id", "uniqueidentifier");
            await AssertColumnDefinition("import_log", "timestamp", "datetime");
            await AssertColumnDefinition("import_log", "log_severity", "nvarchar");
            await AssertColumnDefinition("import_log", "log_message", "nvarchar");
            await AssertColumnDefinition("import_log", "log_exception", "nvarchar");
            await AssertPrimaryKey("import_log", "log_id");
            await AssertForeignKey("import_log", "import_id", "import", "import_id");
            await AssertIndex("import_log", "import_id");
        }
    }
}
