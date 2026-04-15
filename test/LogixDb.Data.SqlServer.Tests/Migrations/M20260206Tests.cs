namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260206Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602061010_CreatesTargetTableWithExpectedColumns()
    {
        await Database.Migrate(202602061010);

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
    public async Task MigrateUp_ToM202602061020_CreatesSnapshotTableWithExpectedColumns()
    {
        await Database.Migrate(202602061020);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("snapshot");

            await AssertColumnDefinition("snapshot", "snapshot_id", "int");
            await AssertColumnDefinition("snapshot", "target_id", "uniqueidentifier");
            await AssertColumnDefinition("snapshot", "version_number", "int");
            await AssertColumnDefinition("snapshot", "target_type", "nvarchar");
            await AssertColumnDefinition("snapshot", "target_name", "nvarchar");
            await AssertColumnDefinition("snapshot", "is_partial", "bit");
            await AssertColumnDefinition("snapshot", "schema_revision", "nvarchar");
            await AssertColumnDefinition("snapshot", "software_revision", "nvarchar");
            await AssertColumnDefinition("snapshot", "export_date", "datetime");
            await AssertColumnDefinition("snapshot", "export_user", "nvarchar");
            await AssertColumnDefinition("snapshot", "export_options", "nvarchar");
            await AssertColumnDefinition("snapshot", "import_date", "datetime");
            await AssertColumnDefinition("snapshot", "import_user", "nvarchar");
            await AssertColumnDefinition("snapshot", "import_machine", "nvarchar");
            await AssertColumnDefinition("snapshot", "source_hash", "nvarchar");
            await AssertColumnDefinition("snapshot", "source_data", "varbinary");

            await AssertPrimaryKey("snapshot", "snapshot_id");
            await AssertForeignKey("snapshot", "target_id", "target", "target_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602061030_CreatesSnapshotPropertyTableWithExpectedColumns()
    {
        await Database.Migrate(202602061030);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("snapshot_property");

            await AssertColumnDefinition("snapshot_property", "property_id", "uniqueidentifier");
            await AssertColumnDefinition("snapshot_property", "snapshot_id", "int");
            await AssertColumnDefinition("snapshot_property", "property_name", "nvarchar");
            await AssertColumnDefinition("snapshot_property", "property_value", "nvarchar");

            await AssertPrimaryKey("snapshot_property", "property_id");
            await AssertForeignKey("snapshot_property", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("snapshot_property", "snapshot_id", "property_name");
        }
    }
}