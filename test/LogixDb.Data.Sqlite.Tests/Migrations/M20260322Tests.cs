namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260322Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM021_CreatesTagProduceInfoTableWithExpectedColumns()
    {
        await Database.Migrate(202603220500);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_produce_info");

            await AssertColumnDefinition("tag_produce_info", "produce_info_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_produce_info", "snapshot_id", "integer");
            await AssertColumnDefinition("tag_produce_info", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_produce_info", "produce_count", "integer");
            await AssertColumnDefinition("tag_produce_info", "programatically_send_event_trigger", "integer");
            await AssertColumnDefinition("tag_produce_info", "unicast_permitted", "integer");
            await AssertColumnDefinition("tag_produce_info", "maximum_rpi", "numeric");
            await AssertColumnDefinition("tag_produce_info", "minimum_rpi", "numeric");
            await AssertColumnDefinition("tag_produce_info", "default_rpi", "numeric");
            await AssertColumnDefinition("tag_produce_info", "record_hash", "text");

            await AssertPrimaryKey("tag_produce_info", "produce_info_id");
            await AssertForeignKey("tag_produce_info", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("tag_produce_info", "snapshot_id", "tag_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM022_CreatesTagConsumeInfoTableWithExpectedColumns()
    {
        await Database.Migrate(202603220530);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_consume_info");

            await AssertColumnDefinition("tag_consume_info", "consume_info_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_consume_info", "snapshot_id", "integer");
            await AssertColumnDefinition("tag_consume_info", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_consume_info", "producer", "text");
            await AssertColumnDefinition("tag_consume_info", "remote_tag", "text");
            await AssertColumnDefinition("tag_consume_info", "remote_instance", "integer");
            await AssertColumnDefinition("tag_consume_info", "rpi", "numeric");
            await AssertColumnDefinition("tag_consume_info", "unicast", "integer");
            await AssertColumnDefinition("tag_consume_info", "record_hash", "text");

            await AssertPrimaryKey("tag_consume_info", "consume_info_id");
            await AssertForeignKey("tag_consume_info", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("tag_consume_info", "snapshot_id", "tag_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM023_CreatesTagAliasTableWithExpectedColumns()
    {
        await Database.Migrate(202603220600);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_alias");

            await AssertColumnDefinition("tag_alias", "alias_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_alias", "snapshot_id", "integer");
            await AssertColumnDefinition("tag_alias", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_alias", "alias_for", "text");

            await AssertPrimaryKey("tag_alias", "alias_id");
            await AssertForeignKey("tag_alias", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("tag_alias", "snapshot_id", "tag_id");
        }
    }
}
