namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260212Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602120830_CreatesAoiTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("aoi");

            await AssertColumnDefinition("aoi", "aoi_id", "bigint");
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
            await AssertColumnDefinition("aoi", "content_hash", "nvarchar");
            await AssertColumnDefinition("aoi", "record_hash", "nvarchar");

            await AssertPrimaryKey("aoi", "aoi_id");
            await AssertUniqueIndex("aoi", "record_hash");
            await AssertIndex("aoi", "aoi_name");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602120900_CreatesAoiParameterTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("aoi_parameter");

            await AssertColumnDefinition("aoi_parameter", "parameter_id", "bigint");
            await AssertColumnDefinition("aoi_parameter", "aoi_id", "bigint");
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
            await AssertUniqueIndex("aoi_parameter", "aoi_id", "record_hash");
            await AssertUniqueIndex("aoi_parameter", "aoi_id", "parameter_name");
            await AssertIndex("aoi_parameter", "parameter_name");
        }
    }
}