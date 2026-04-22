namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260206Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602060900_CreatesTargetTableWithExpectedColumns()
    {
        await Database.Migrate(202602060900);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("target");

            await AssertColumnDefinition("target", "target_id", "uniqueidentifier");
            await AssertColumnDefinition("target", "target_key", "nvarchar");
            await AssertColumnDefinition("target", "created_on", "datetime");

            await AssertPrimaryKey("target", "target_id");
            await AssertUniqueIndex("target", "target_key");
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
            await AssertColumnDefinition("target_version", "version_number", "int");
            await AssertColumnDefinition("target_version", "target_type", "nvarchar");
            await AssertColumnDefinition("target_version", "target_name", "nvarchar");
            await AssertColumnDefinition("target_version", "is_partial", "bit");
            await AssertColumnDefinition("target_version", "schema_revision", "nvarchar");
            await AssertColumnDefinition("target_version", "software_revision", "nvarchar");
            await AssertColumnDefinition("target_version", "export_date", "datetime");
            await AssertColumnDefinition("target_version", "export_options", "nvarchar");
            await AssertColumnDefinition("target_version", "import_date", "datetime");
            await AssertColumnDefinition("target_version", "import_user", "nvarchar");
            await AssertColumnDefinition("target_version", "import_machine", "nvarchar");
            await AssertColumnDefinition("target_version", "source_hash", "nvarchar");
            await AssertColumnDefinition("target_version", "source_data", "varbinary");

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

            await AssertColumnDefinition("target_instance", "instance_id", "int");
            await AssertColumnDefinition("target_instance", "version_id", "uniqueidentifier");
            await AssertColumnDefinition("target_instance", "restored_on", "datetime");
            await AssertColumnDefinition("target_instance", "restored_by", "nvarchar");

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
            await AssertColumnDefinition("target_info", "property_name", "nvarchar");
            await AssertColumnDefinition("target_info", "property_value", "nvarchar");

            await AssertPrimaryKey("target_info", "property_id");
            await AssertForeignKey("target_info", "version_id", "target_version", "version_id");
            await AssertUniqueIndex("target_info", "version_id", "property_name");
        }
    }
}