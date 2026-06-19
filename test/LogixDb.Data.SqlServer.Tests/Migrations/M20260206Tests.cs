namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260206Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602060900_CreatesTargetTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "target");

            await AssertColumnDefinition("target", "target_id", "int");
            await AssertColumnDefinition("target", "target_key", "nvarchar");
            await AssertColumnDefinition("target", "created_on", "datetime");

            await AssertPrimaryKey("target", "target_id");
            await AssertUniqueIndex("target", "target_key");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602061000_CreatesVersionTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "target_version");

            await AssertColumnDefinition("target_version", "version_id", "int");
            await AssertColumnDefinition("target_version", "target_id", "int");
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
    public async Task MigrateUp_ToM202602061030_CreatesVersionMapTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "target_version_map");

            await AssertColumnDefinition("target_version_map", "version_id", "int");
            await AssertColumnDefinition("target_version_map", "record_id", "bigint");
            await AssertColumnDefinition("target_version_map", "component_id", "tinyint");

            await AssertForeignKey("target_version_map", "version_id", "target_version", "version_id");
            await AssertUniqueIndex("target_version_map", "version_id", "component_id", "record_id");
            await AssertIndex("target_version_map", "record_id", "component_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602061100_CreatesTargetInfoTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "target_info");

            await AssertColumnDefinition("target_info", "property_id", "uniqueidentifier");
            await AssertColumnDefinition("target_info", "version_id", "int");
            await AssertColumnDefinition("target_info", "property_name", "nvarchar");
            await AssertColumnDefinition("target_info", "property_value", "nvarchar");

            await AssertPrimaryKey("target_info", "property_id");
            await AssertForeignKey("target_info", "version_id", "target_version", "version_id");
            await AssertUniqueIndex("target_info", "version_id", "property_name");
        }
    }
    
    [Test]
    public async Task MigrateUp_ToM202602061130_CreatesComponentTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "target_component");

            await AssertColumnDefinition("target_component", "component_id", "tinyint");
            await AssertColumnDefinition("target_component", "component_name", "nvarchar");

            await AssertPrimaryKey("target_component", "component_id");
            await AssertUniqueIndex("target_component", "component_name");
        }
    }
}