namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260212Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602120830_CreatesAoiTableWithExpectedColumns()
    {
        await Database.Migrate(202602120830);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("aoi");

            await AssertColumnDefinition("aoi", "aoi_id", "uniqueidentifier");
            await AssertColumnDefinition("aoi", "snapshot_id", "int");
            await AssertColumnDefinition("aoi", "aoi_name", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_description", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_revision", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_revision_extension", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_revision_note", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_vendor", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_help_text", "nvarchar");
            await AssertColumnDefinition("aoi", "created_date", "datetime");
            await AssertColumnDefinition("aoi", "created_by", "nvarchar");
            await AssertColumnDefinition("aoi", "edited_date", "datetime");
            await AssertColumnDefinition("aoi", "edited_by", "nvarchar");
            await AssertColumnDefinition("aoi", "software_revision", "nvarchar");
            await AssertColumnDefinition("aoi", "execute_pre_scan", "bit");
            await AssertColumnDefinition("aoi", "execute_post_scan", "bit");
            await AssertColumnDefinition("aoi", "execute_enable_in_false", "bit");
            await AssertColumnDefinition("aoi", "is_encrypted", "bit");
            await AssertColumnDefinition("aoi", "signature_id", "nvarchar");
            await AssertColumnDefinition("aoi", "signature_timestamp", "datetime");
            await AssertColumnDefinition("aoi", "component_class", "nvarchar");
            await AssertColumnDefinition("aoi", "record_hash", "nvarchar");
            await AssertColumnDefinition("aoi", "source_hash", "nvarchar");

            await AssertPrimaryKey("aoi", "aoi_id");
            await AssertForeignKey("aoi", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("aoi", "snapshot_id", "aoi_name");
            await AssertIndex("aoi", "aoi_name", "record_hash");
            await AssertIndex("aoi", "source_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602120900_CreatesAoiParameterTableWithExpectedColumns()
    {
        await Database.Migrate(202602120900);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("aoi_parameter");

            await AssertColumnDefinition("aoi_parameter", "parameter_id", "uniqueidentifier");
            await AssertColumnDefinition("aoi_parameter", "snapshot_id", "int");
            await AssertColumnDefinition("aoi_parameter", "aoi_id", "uniqueidentifier");
            await AssertColumnDefinition("aoi_parameter", "parameter_name", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "parameter_description", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "data_type", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "dimensions", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "radix", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "default_value", "nvarchar");
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
            await AssertForeignKey("aoi_parameter", "aoi_id", "aoi", "aoi_id");
            await AssertUniqueIndex("aoi_parameter", "aoi_id", "parameter_name");
            await AssertIndex("aoi_parameter", "parameter_name", "record_hash");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602120910_CreatesAoiRungTableWithExpectedColumns()
    {
        await Database.Migrate(202602120910);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("aoi_rung");

            await AssertColumnDefinition("aoi_rung", "rung_id", "uniqueidentifier");
            await AssertColumnDefinition("aoi_rung", "snapshot_id", "int");
            await AssertColumnDefinition("aoi_rung", "aoi_id", "uniqueidentifier");
            await AssertColumnDefinition("aoi_rung", "routine_name", "nvarchar");
            await AssertColumnDefinition("aoi_rung", "rung_number", "int");
            await AssertColumnDefinition("aoi_rung", "rung_text", "nvarchar");
            await AssertColumnDefinition("aoi_rung", "rung_comment", "nvarchar");
            await AssertColumnDefinition("aoi_rung", "record_hash", "nvarchar");

            await AssertPrimaryKey("aoi_rung", "rung_id");
            await AssertForeignKey("aoi_rung", "snapshot_id", "snapshot", "snapshot_id");
            await AssertForeignKey("aoi_rung", "aoi_id", "aoi", "aoi_id");
            await AssertUniqueIndex("aoi_rung", "aoi_id", "routine_name", "rung_number");
            await AssertIndex("aoi_rung", "record_hash", "aoi_id");
        }
    }
}