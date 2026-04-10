namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260213Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602130830_CreatesTagTableWithExpectedColumns()
    {
        await Database.Migrate(202602130830);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag");

            await AssertColumnDefinition("tag", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag", "snapshot_id", "int");
            await AssertColumnDefinition("tag", "program_id", "uniqueidentifier");
            await AssertColumnDefinition("tag", "tag_name", "nvarchar");
            await AssertColumnDefinition("tag", "data_type", "nvarchar");
            await AssertColumnDefinition("tag", "dimensions", "nvarchar");
            await AssertColumnDefinition("tag", "radix", "nvarchar");
            await AssertColumnDefinition("tag", "external_access", "nvarchar");
            await AssertColumnDefinition("tag", "opcua_access", "nvarchar");
            await AssertColumnDefinition("tag", "is_constant", "bit");
            await AssertColumnDefinition("tag", "tag_usage", "nvarchar");
            await AssertColumnDefinition("tag", "tag_type", "nvarchar");
            await AssertColumnDefinition("tag", "record_hash", "nvarchar");
            await AssertColumnDefinition("tag", "source_hash", "nvarchar");

            await AssertPrimaryKey("tag", "tag_id");
            await AssertForeignKey("tag", "snapshot_id", "snapshot", "snapshot_id");
            await AssertForeignKey("tag", "program_id", "program", "program_id");
            await AssertUniqueIndex("tag", "snapshot_id", "program_id", "tag_name");
            await AssertIndex("tag", "tag_name", "snapshot_id");
            await AssertIndex("tag", "data_type", "snapshot_id");
            await AssertIndex("tag", "record_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602130900_CreatesTagMemberTableWithExpectedColumns()
    {
        await Database.Migrate(202602130900);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_member");

            await AssertColumnDefinition("tag_member", "member_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_member", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_member", "parent_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_member", "tag_name", "nvarchar");
            await AssertColumnDefinition("tag_member", "member_name", "nvarchar");
            await AssertColumnDefinition("tag_member", "data_type", "nvarchar");
            await AssertColumnDefinition("tag_member", "tag_value", "nvarchar");

            await AssertPrimaryKey("tag_member", "member_id");
            await AssertForeignKey("tag_member", "tag_id", "tag", "tag_id");
            await AssertForeignKey("tag_member", "parent_id", "tag_member", "member_id");
            await AssertUniqueIndex("tag_member", "tag_id", "tag_name");
            await AssertIndex("tag_member", "parent_id", "member_name");
            await AssertIndex("tag_member", "tag_name", "tag_id");
            await AssertIndex("tag_member", "data_type", "tag_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602131200_CreatesTagCommentTableWithExpectedColumns()
    {
        await Database.Migrate(202602131200);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_comment");

            await AssertColumnDefinition("tag_comment", "comment_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_comment", "member_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_comment", "tag_name", "nvarchar");
            await AssertColumnDefinition("tag_comment", "tag_comment", "nvarchar");
            await AssertColumnDefinition("tag_comment", "record_hash", "nvarchar");

            await AssertPrimaryKey("tag_comment", "comment_id");
            await AssertForeignKey("tag_comment", "member_id", "tag_member", "member_id");
            await AssertUniqueIndex("tag_comment", "member_id", "tag_name");
            await AssertIndex("tag_comment", "tag_name", "member_id");
            await AssertIndex("tag_comment", "record_hash", "tag_name");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602131300_CreatesTagProducerTableWithExpectedColumns()
    {
        await Database.Migrate(202602131300);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_producer");

            await AssertColumnDefinition("tag_producer", "producer_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_producer", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_producer", "produce_count", "int");
            await AssertColumnDefinition("tag_producer", "send_event_trigger", "bit");
            await AssertColumnDefinition("tag_producer", "unicast_permitted", "bit");
            await AssertColumnDefinition("tag_producer", "maximum_rpi", "float");
            await AssertColumnDefinition("tag_producer", "minimum_rpi", "float");
            await AssertColumnDefinition("tag_producer", "default_rpi", "float");
            await AssertColumnDefinition("tag_producer", "record_hash", "nvarchar");

            await AssertPrimaryKey("tag_producer", "producer_id");
            await AssertForeignKey("tag_producer", "tag_id", "tag", "tag_id");
            await AssertUniqueIndex("tag_producer", "tag_id");
            await AssertIndex("tag_producer", "tag_id", "record_hash");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602131330_CreatesTagConsumerTableWithExpectedColumns()
    {
        await Database.Migrate(202602131330);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_consumer");

            await AssertColumnDefinition("tag_consumer", "consumer_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_consumer", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_consumer", "producer", "nvarchar");
            await AssertColumnDefinition("tag_consumer", "remote_tag", "nvarchar");
            await AssertColumnDefinition("tag_consumer", "remote_instance", "int");
            await AssertColumnDefinition("tag_consumer", "rpi", "float");
            await AssertColumnDefinition("tag_consumer", "unicast", "bit");
            await AssertColumnDefinition("tag_consumer", "record_hash", "nvarchar");

            await AssertPrimaryKey("tag_consumer", "consumer_id");
            await AssertForeignKey("tag_consumer", "tag_id", "tag", "tag_id");
            await AssertUniqueIndex("tag_consumer", "tag_id");
            await AssertIndex("tag_consumer", "tag_id", "record_hash");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602131400_CreatesTagAliasTableWithExpectedColumns()
    {
        await Database.Migrate(202602131400);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_alias");

            await AssertColumnDefinition("tag_alias", "alias_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_alias", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_alias", "alias_for", "nvarchar");

            await AssertPrimaryKey("tag_alias", "alias_id");
            await AssertForeignKey("tag_alias", "tag_id", "tag", "tag_id");
            await AssertUniqueIndex("tag_alias", "tag_id");
            await AssertIndex("tag_alias", "alias_for");
        }
    }
}