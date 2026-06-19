namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260616Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202606162100_CreatesTraceabilityTablesWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            // Assert import table
            await AssertTableExists("import");
            await AssertColumnDefinition("import", "import_id", "UNIQUEIDENTIFIER");
            await AssertColumnDefinition("import", "import_status", "text");
            await AssertColumnDefinition("import", "source_type", "text");
            await AssertColumnDefinition("import", "file_type", "text");
            await AssertColumnDefinition("import", "file_name", "text");
            await AssertColumnDefinition("import", "posted_on", "datetime");
            await AssertPrimaryKey("import", "import_id");

            // Assert import_log table
            await AssertTableExists("import_log");
            await AssertColumnDefinition("import_log", "log_id", "integer");
            await AssertColumnDefinition("import_log", "import_id", "UNIQUEIDENTIFIER");
            await AssertColumnDefinition("import_log", "timestamp", "datetime");
            await AssertColumnDefinition("import_log", "log_severity", "text");
            await AssertColumnDefinition("import_log", "log_message", "text");
            await AssertColumnDefinition("import_log", "log_exception", "text");
            await AssertPrimaryKey("import_log", "log_id");
            await AssertForeignKey("import_log", "import_id", "import", "import_id");
            await AssertIndex("import_log", "import_id");
        }
    }
}
