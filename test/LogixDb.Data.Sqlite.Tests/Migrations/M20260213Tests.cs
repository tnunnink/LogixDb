namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260213Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602130830_CreatesTagTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag");

            await AssertColumnDefinition("tag", "tag_id", "integer");
            await AssertColumnDefinition("tag", "program_name", "text");
            await AssertColumnDefinition("tag", "tag_name", "text");
            await AssertColumnDefinition("tag", "tag_description", "text");
            await AssertColumnDefinition("tag", "data_type", "text");
            await AssertColumnDefinition("tag", "dimensions", "text");
            await AssertColumnDefinition("tag", "radix", "text");
            await AssertColumnDefinition("tag", "external_access", "text");
            await AssertColumnDefinition("tag", "opcua_access", "text");
            await AssertColumnDefinition("tag", "is_constant", "integer");
            await AssertColumnDefinition("tag", "tag_usage", "text");
            await AssertColumnDefinition("tag", "tag_type", "text");
            await AssertColumnDefinition("tag", "alias_for", "text");
            await AssertColumnDefinition("tag", "content_hash", "text");
            await AssertColumnDefinition("tag", "record_hash", "text");

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
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_member");

            await AssertColumnDefinition("tag_member", "member_id", "integer");
            await AssertColumnDefinition("tag_member", "tag_id", "integer");
            await AssertColumnDefinition("tag_member", "member_path", "text");
            await AssertColumnDefinition("tag_member", "parent_path", "text");
            await AssertColumnDefinition("tag_member", "member_name", "text");
            await AssertColumnDefinition("tag_member", "data_type", "text");

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
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_value");

            await AssertColumnDefinition("tag_value", "version_id", "integer");
            await AssertColumnDefinition("tag_value", "member_id", "integer");
            await AssertColumnDefinition("tag_value", "tag_value", "text");

            await AssertForeignKey("tag_value", "version_id", "target_version", "version_id");
            await AssertForeignKey("tag_value", "member_id", "tag_member", "member_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602131200_CreatesTagCommentTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_member_comment");

            await AssertColumnDefinition("tag_member_comment", "tag_id", "integer");
            await AssertColumnDefinition("tag_member_comment", "member_path", "text");
            await AssertColumnDefinition("tag_member_comment", "comment", "text");
            await AssertColumnDefinition("tag_member_comment", "record_hash", "text");

            await AssertForeignKey("tag_member_comment", "tag_id", "tag", "tag_id");
            await AssertUniqueIndex("tag_member_comment", "tag_id", "record_hash");
            await AssertUniqueIndex("tag_member_comment", "tag_id", "member_path");
            await AssertIndex("tag_member_comment", "member_path");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602131300_CreatesTagProducerTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_producer");

            await AssertColumnDefinition("tag_producer", "tag_id", "integer");
            await AssertColumnDefinition("tag_producer", "produce_count", "integer");
            await AssertColumnDefinition("tag_producer", "send_event_trigger", "integer");
            await AssertColumnDefinition("tag_producer", "unicast_permitted", "integer");
            await AssertColumnDefinition("tag_producer", "maximum_rpi", "numeric");
            await AssertColumnDefinition("tag_producer", "minimum_rpi", "numeric");
            await AssertColumnDefinition("tag_producer", "default_rpi", "numeric");
            await AssertColumnDefinition("tag_producer", "record_hash", "text");

            await AssertForeignKey("tag_producer", "tag_id", "tag", "tag_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602131330_CreatesTagConsumerTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_consumer");

            await AssertColumnDefinition("tag_consumer", "tag_id", "integer");
            await AssertColumnDefinition("tag_consumer", "producer", "text");
            await AssertColumnDefinition("tag_consumer", "remote_tag", "text");
            await AssertColumnDefinition("tag_consumer", "remote_instance", "integer");
            await AssertColumnDefinition("tag_consumer", "rpi", "numeric");
            await AssertColumnDefinition("tag_consumer", "unicast", "integer");
            await AssertColumnDefinition("tag_consumer", "record_hash", "text");

            await AssertForeignKey("tag_consumer", "tag_id", "tag", "tag_id");
        }
    }

}