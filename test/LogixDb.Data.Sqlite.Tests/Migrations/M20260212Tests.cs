namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260212Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602120830_CreatesAoiTableWithExpectedColumns()
    {
        await Database.Migrate(202602120830);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("aoi");

            await AssertColumnDefinition("aoi", "aoi_id", "uniqueidentifier");
            await AssertColumnDefinition("aoi", "instance_id", "integer");
            await AssertColumnDefinition("aoi", "aoi_name", "text");
            await AssertColumnDefinition("aoi", "aoi_description", "text");
            await AssertColumnDefinition("aoi", "aoi_revision", "text");
            await AssertColumnDefinition("aoi", "aoi_revision_extension", "text");
            await AssertColumnDefinition("aoi", "aoi_revision_note", "text");
            await AssertColumnDefinition("aoi", "aoi_vendor", "text");
            await AssertColumnDefinition("aoi", "aoi_help_text", "text");
            await AssertColumnDefinition("aoi", "created_date", "datetime");
            await AssertColumnDefinition("aoi", "created_by", "text");
            await AssertColumnDefinition("aoi", "edited_date", "datetime");
            await AssertColumnDefinition("aoi", "edited_by", "text");
            await AssertColumnDefinition("aoi", "software_revision", "text");
            await AssertColumnDefinition("aoi", "execute_pre_scan", "integer");
            await AssertColumnDefinition("aoi", "execute_post_scan", "integer");
            await AssertColumnDefinition("aoi", "execute_enable_in_false", "integer");
            await AssertColumnDefinition("aoi", "is_encrypted", "integer");
            await AssertColumnDefinition("aoi", "signature_id", "text");
            await AssertColumnDefinition("aoi", "signature_timestamp", "datetime");
            await AssertColumnDefinition("aoi", "component_class", "text");
            await AssertColumnDefinition("aoi", "record_hash", "text");
            await AssertColumnDefinition("aoi", "source_hash", "text");

            await AssertPrimaryKey("aoi", "aoi_id");
            await AssertForeignKey("aoi", "instance_id", "target_instance", "instance_id");
            await AssertUniqueIndex("aoi", "instance_id", "aoi_name");
            await AssertIndex("aoi", "aoi_name", "record_hash");
            await AssertIndex("aoi", "source_hash", "instance_id");
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
            await AssertColumnDefinition("aoi_parameter", "instance_id", "integer");
            await AssertColumnDefinition("aoi_parameter", "aoi_id", "uniqueidentifier");
            await AssertColumnDefinition("aoi_parameter", "parameter_name", "text");
            await AssertColumnDefinition("aoi_parameter", "parameter_description", "text");
            await AssertColumnDefinition("aoi_parameter", "data_type", "text");
            await AssertColumnDefinition("aoi_parameter", "dimensions", "text");
            await AssertColumnDefinition("aoi_parameter", "radix", "text");
            await AssertColumnDefinition("aoi_parameter", "default_value", "text");
            await AssertColumnDefinition("aoi_parameter", "external_access", "text");
            await AssertColumnDefinition("aoi_parameter", "tag_usage", "text");
            await AssertColumnDefinition("aoi_parameter", "tag_type", "text");
            await AssertColumnDefinition("aoi_parameter", "tag_alias", "text");
            await AssertColumnDefinition("aoi_parameter", "is_visible", "integer");
            await AssertColumnDefinition("aoi_parameter", "is_required", "integer");
            await AssertColumnDefinition("aoi_parameter", "is_constant", "integer");
            await AssertColumnDefinition("aoi_parameter", "record_hash", "text");

            await AssertPrimaryKey("aoi_parameter", "parameter_id");
            await AssertForeignKey("aoi_parameter", "instance_id", "target_instance", "instance_id");
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
            await AssertColumnDefinition("aoi_rung", "instance_id", "integer");
            await AssertColumnDefinition("aoi_rung", "aoi_id", "uniqueidentifier");
            await AssertColumnDefinition("aoi_rung", "routine_name", "text");
            await AssertColumnDefinition("aoi_rung", "rung_number", "integer");
            await AssertColumnDefinition("aoi_rung", "rung_text", "text");
            await AssertColumnDefinition("aoi_rung", "rung_comment", "text");
            await AssertColumnDefinition("aoi_rung", "record_hash", "text");

            await AssertPrimaryKey("aoi_rung", "rung_id");
            await AssertForeignKey("aoi_rung", "instance_id", "target_instance", "instance_id");
            await AssertForeignKey("aoi_rung", "aoi_id", "aoi", "aoi_id");
            await AssertUniqueIndex("aoi_rung", "aoi_id", "routine_name", "rung_number");
            await AssertIndex("aoi_rung", "record_hash", "aoi_id");
        }
    }
}