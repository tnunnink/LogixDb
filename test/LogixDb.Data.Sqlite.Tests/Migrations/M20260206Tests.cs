namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260206Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602060900_CreatesTargetTableWithExpectedColumns()
    {
        await Database.Migrate(202602060900);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("target");

            await AssertColumnDefinition("target", "target_id", "uniqueidentifier");
            await AssertColumnDefinition("target", "target_key", "text");
            await AssertColumnDefinition("target", "created_on", "datetime");

            await AssertPrimaryKey("target", "target_id");
            await AssertIndex("target", "target_key");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602061000_CreatesTargetArchiveTableWithExpectedColumns()
    {
        await Database.Migrate(202602061000);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("target_version");

            await AssertColumnDefinition("target_version", "version_id", "uniqueidentifier");
            await AssertColumnDefinition("target_version", "target_id", "uniqueidentifier");
            await AssertColumnDefinition("target_version", "version_number", "integer");
            await AssertColumnDefinition("target_version", "target_type", "text");
            await AssertColumnDefinition("target_version", "target_name", "text");
            await AssertColumnDefinition("target_version", "is_partial", "integer");
            await AssertColumnDefinition("target_version", "schema_revision", "text");
            await AssertColumnDefinition("target_version", "software_revision", "text");
            await AssertColumnDefinition("target_version", "export_date", "datetime");
            await AssertColumnDefinition("target_version", "export_options", "text");
            await AssertColumnDefinition("target_version", "import_date", "datetime");
            await AssertColumnDefinition("target_version", "import_user", "text");
            await AssertColumnDefinition("target_version", "import_machine", "text");
            await AssertColumnDefinition("target_version", "source_hash", "text");
            await AssertColumnDefinition("target_version", "source_data", "blob");

            await AssertPrimaryKey("target_version", "version_id");
            await AssertForeignKey("target_version", "target_id", "target", "target_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602061030_CreatesTargetInstanceTableWithExpectedColumns()
    {
        await Database.Migrate(202602061030);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("target_instance");

            await AssertColumnDefinition("target_instance", "instance_id", "integer");
            await AssertColumnDefinition("target_instance", "version_id", "uniqueidentifier");
            await AssertColumnDefinition("target_instance", "restored_on", "datetime");
            await AssertColumnDefinition("target_instance", "restored_by", "text");

            await AssertPrimaryKey("target_instance", "instance_id");
            await AssertForeignKey("target_instance", "version_id", "target_version", "version_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602061100_CreatesTargetInfoTableWithExpectedColumns()
    {
        await Database.Migrate(202602061100);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("target_info");

            await AssertColumnDefinition("target_info", "property_id", "uniqueidentifier");
            await AssertColumnDefinition("target_info", "version_id", "uniqueidentifier");
            await AssertColumnDefinition("target_info", "property_name", "text");
            await AssertColumnDefinition("target_info", "property_value", "text");

            await AssertPrimaryKey("target_info", "property_id");
            await AssertForeignKey("target_info", "version_id", "target_version", "version_id");
            await AssertUniqueIndex("target_info", "version_id", "property_name");
        }
    }
}