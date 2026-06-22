namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260213Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602130830_CreatesTagTableWithExpectedColumns()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag");

            await AssertColumnDefinition("tag", "tag_id", "bigint");
            await AssertColumnDefinition("tag", "program_name", "nvarchar");
            await AssertColumnDefinition("tag", "tag_name", "nvarchar");
            await AssertColumnDefinition("tag", "tag_description", "nvarchar");
            await AssertColumnDefinition("tag", "data_type", "nvarchar");
            await AssertColumnDefinition("tag", "dimensions", "nvarchar");
            await AssertColumnDefinition("tag", "radix", "nvarchar");
            await AssertColumnDefinition("tag", "external_access", "nvarchar");
            await AssertColumnDefinition("tag", "opcua_access", "nvarchar");
            await AssertColumnDefinition("tag", "is_constant", "bit");
            await AssertColumnDefinition("tag", "tag_usage", "nvarchar");
            await AssertColumnDefinition("tag", "tag_type", "nvarchar");
            await AssertColumnDefinition("tag", "alias_for", "nvarchar");
            await AssertColumnDefinition("tag", "content_hash", "nvarchar");
            await AssertColumnDefinition("tag", "record_hash", "nvarchar");

            await AssertPrimaryKey("tag", "tag_id");
            await AssertUniqueIndex("tag", "record_hash");
            await AssertIndex("tag", "program_name", "tag_name");
            await AssertIndex("tag", "tag_name");
            await AssertIndex("tag", "data_type");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602130900_CreatesTagMemberTableWithExpectedColumns()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag_member");

            await AssertColumnDefinition("tag_member", "member_id", "bigint");
            await AssertColumnDefinition("tag_member", "tag_id", "bigint");
            await AssertColumnDefinition("tag_member", "member_path", "nvarchar");
            await AssertColumnDefinition("tag_member", "parent_path", "nvarchar");
            await AssertColumnDefinition("tag_member", "member_name", "nvarchar");
            await AssertColumnDefinition("tag_member", "data_type", "nvarchar");

            await AssertPrimaryKey("tag_member", "member_id");
            await AssertForeignKey("tag_member", "tag_id", "tag", "tag_id");
            await AssertUniqueIndex("tag_member", "tag_id", "member_path");
            await AssertIndex("tag_member", "member_path");
            await AssertIndex("tag_member", "parent_path", "tag_id");
            await AssertIndex("tag_member", "member_name", "tag_id");
            await AssertIndex("tag_member", "data_type", "tag_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602130930_CreatesTagValueTableWithExpectedColumns()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag_value");

            await AssertColumnDefinition("tag_value", "version_id", "int");
            await AssertColumnDefinition("tag_value", "member_id", "bigint");
            await AssertColumnDefinition("tag_value", "tag_value", "nvarchar");

            await AssertForeignKey("tag_value", "version_id", "target_version", "version_id");
            await AssertForeignKey("tag_value", "member_id", "tag_member", "member_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602131200_CreatesTagCommentTableWithExpectedColumns()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag_member_comment");

            await AssertColumnDefinition("tag_member_comment", "tag_id", "bigint");
            await AssertColumnDefinition("tag_member_comment", "member_path", "nvarchar");
            await AssertColumnDefinition("tag_member_comment", "comment", "nvarchar");
            await AssertColumnDefinition("tag_member_comment", "record_hash", "nvarchar");

            await AssertForeignKey("tag_member_comment", "tag_id", "tag", "tag_id");
            await AssertUniqueIndex("tag_member_comment", "tag_id", "record_hash");
            await AssertUniqueIndex("tag_member_comment", "tag_id", "member_path");
            await AssertIndex("tag_member_comment", "member_path");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602131300_CreatesTagProducerTableWithExpectedColumns()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag_producer");

            await AssertColumnDefinition("tag_producer", "tag_id", "bigint");
            await AssertColumnDefinition("tag_producer", "produce_count", "int");
            await AssertColumnDefinition("tag_producer", "send_event_trigger", "bit");
            await AssertColumnDefinition("tag_producer", "unicast_permitted", "bit");
            await AssertColumnDefinition("tag_producer", "maximum_rpi", "float");
            await AssertColumnDefinition("tag_producer", "minimum_rpi", "float");
            await AssertColumnDefinition("tag_producer", "default_rpi", "float");
            await AssertColumnDefinition("tag_producer", "record_hash", "nvarchar");

            await AssertForeignKey("tag_producer", "tag_id", "tag", "tag_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602131330_CreatesTagConsumerTableWithExpectedColumns()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag_consumer");

            await AssertColumnDefinition("tag_consumer", "tag_id", "bigint");
            await AssertColumnDefinition("tag_consumer", "producer", "nvarchar");
            await AssertColumnDefinition("tag_consumer", "remote_tag", "nvarchar");
            await AssertColumnDefinition("tag_consumer", "remote_instance", "int");
            await AssertColumnDefinition("tag_consumer", "rpi", "float");
            await AssertColumnDefinition("tag_consumer", "unicast", "bit");
            await AssertColumnDefinition("tag_consumer", "record_hash", "nvarchar");

            await AssertForeignKey("tag_consumer", "tag_id", "tag", "tag_id");
        }
    }
}