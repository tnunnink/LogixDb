namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260212Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM011_CreatesAoiTableWithExpectedColumns()
    {
        await Database.Migrate(202602120830);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("aoi");

            await AssertColumnDefinition("aoi", "aoi_id", "int");
            await AssertColumnDefinition("aoi", "snapshot_id", "int");
            await AssertColumnDefinition("aoi", "aoi_name", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_revision", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_revision_extension", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_revision_note", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_vendor", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_description", "nvarchar");
            await AssertColumnDefinition("aoi", "execute_pre_scan", "bit");
            await AssertColumnDefinition("aoi", "execute_post_scan", "bit");
            await AssertColumnDefinition("aoi", "execute_enable_in_false", "bit");
            await AssertColumnDefinition("aoi", "created_date", "datetime");
            await AssertColumnDefinition("aoi", "created_by", "nvarchar");
            await AssertColumnDefinition("aoi", "edited_date", "datetime");
            await AssertColumnDefinition("aoi", "edited_by", "nvarchar");
            await AssertColumnDefinition("aoi", "software_revision", "nvarchar");
            await AssertColumnDefinition("aoi", "help_text", "nvarchar");
            await AssertColumnDefinition("aoi", "is_encrypted", "bit");
            await AssertColumnDefinition("aoi", "signature_id", "nvarchar");
            await AssertColumnDefinition("aoi", "signature_timestamp", "datetime");
            await AssertColumnDefinition("aoi", "component_class", "nvarchar");
            await AssertColumnDefinition("aoi", "record_hash", "nvarchar");

            await AssertPrimaryKey("aoi", "aoi_id");
            await AssertForeignKey("aoi", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("aoi", "snapshot_id", "aoi_name");
            await AssertIndex("aoi", "record_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM012_CreatesAoiParameterTableWithExpectedColumns()
    {
        await Database.Migrate(202602120900);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("aoi_parameter");

            await AssertColumnDefinition("aoi_parameter", "parameter_id", "int");
            await AssertColumnDefinition("aoi_parameter", "snapshot_id", "int");
            await AssertColumnDefinition("aoi_parameter", "aoi_name", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "parameter_name", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "data_type", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "default_value", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "parameter_description", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "external_access", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "tag_usage", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "tag_type", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "tag_alias", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "is_visible", "bit");
            await AssertColumnDefinition("aoi_parameter", "is_required", "bit");
            await AssertColumnDefinition("aoi_parameter", "is_constant", "bit");
            await AssertColumnDefinition("aoi_parameter", "record_hash", "nvarchar");

            await AssertPrimaryKey("aoi_parameter", "parameter_id");
            await AssertForeignKey("aoi_parameter", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("aoi_parameter", "snapshot_id", "aoi_name", "parameter_name");
        }
    }

    [Test]
    public async Task MigrateUp_ToM013_CreatesModuleTableWithExpectedColumns()
    {
        await Database.Migrate(202602120930);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("module");

            await AssertColumnDefinition("module", "module_id", "int");
            await AssertColumnDefinition("module", "snapshot_id", "int");
            await AssertColumnDefinition("module", "module_name", "nvarchar");
            await AssertColumnDefinition("module", "catalog_number", "nvarchar");
            await AssertColumnDefinition("module", "module_revision", "nvarchar");
            await AssertColumnDefinition("module", "module_description", "nvarchar");
            await AssertColumnDefinition("module", "vendor_id", "int");
            await AssertColumnDefinition("module", "product_id", "int");
            await AssertColumnDefinition("module", "product_code", "smallint");
            await AssertColumnDefinition("module", "parent_name", "nvarchar");
            await AssertColumnDefinition("module", "parent_port", "tinyint");
            await AssertColumnDefinition("module", "electronic_keying", "nvarchar");
            await AssertColumnDefinition("module", "is_inhibited", "bit");
            await AssertColumnDefinition("module", "is_major_fault_enabled", "bit");
            await AssertColumnDefinition("module", "is_safety_enabled", "bit");
            await AssertColumnDefinition("module", "ip_address", "nvarchar");
            await AssertColumnDefinition("module", "slot_number", "tinyint");
            await AssertColumnDefinition("module", "record_hash", "nvarchar");

            await AssertPrimaryKey("module", "module_id");
            await AssertForeignKey("module", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("module", "snapshot_id", "module_name");
            await AssertIndex("module", "record_hash", "snapshot_id");
        }
    }
}