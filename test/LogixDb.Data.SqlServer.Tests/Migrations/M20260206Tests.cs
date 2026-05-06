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
    public async Task MigrateUp_ToM202602061000_CreatesVersionTableWithExpectedColumns()
    {
        await Database.Migrate(202602061000);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("version");

            await AssertColumnDefinition("version", "version_id", "uniqueidentifier");
            await AssertColumnDefinition("version", "target_id", "uniqueidentifier");
            await AssertColumnDefinition("version", "version_number", "int");
            await AssertColumnDefinition("version", "target_type", "nvarchar");
            await AssertColumnDefinition("version", "target_name", "nvarchar");
            await AssertColumnDefinition("version", "is_partial", "bit");
            await AssertColumnDefinition("version", "schema_revision", "nvarchar");
            await AssertColumnDefinition("version", "software_revision", "nvarchar");
            await AssertColumnDefinition("version", "export_date", "datetime");
            await AssertColumnDefinition("version", "export_options", "nvarchar");
            await AssertColumnDefinition("version", "import_date", "datetime");
            await AssertColumnDefinition("version", "import_user", "nvarchar");
            await AssertColumnDefinition("version", "import_machine", "nvarchar");
            await AssertColumnDefinition("version", "source_hash", "nvarchar");
            await AssertColumnDefinition("version", "source_data", "varbinary");

            await AssertPrimaryKey("version", "version_id");
            await AssertForeignKey("version", "target_id", "target", "target_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602061030_CreatesVersionMapTableWithExpectedColumns()
    {
        await Database.Migrate(202602061030);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("version_map");

            await AssertColumnDefinition("version_map", "version_id", "uniqueidentifier");
            await AssertColumnDefinition("version_map", "component_id", "uniqueidentifier");
            await AssertColumnDefinition("version_map", "component_type", "nvarchar");

            await AssertForeignKey("version_map", "version_id", "version", "version_id");
            await AssertUniqueIndex("version_map", "version_id", "component_id", "component_type");
            await AssertIndex("version_map", "component_id", "component_type");
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
            await AssertForeignKey("target_info", "version_id", "version", "version_id");
            await AssertUniqueIndex("target_info", "version_id", "property_name");
        }
    }
}