namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260322Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM021_CreatesTagProduceInfoTableWithExpectedColumns()
    {
        await Database.Migrate(202603220500);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_produce_info");

            await AssertColumnDefinition("tag_produce_info", "produce_info_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_produce_info", "snapshot_id", "int");
            await AssertColumnDefinition("tag_produce_info", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_produce_info", "produce_count", "int");
            await AssertColumnDefinition("tag_produce_info", "programatically_send_event_trigger", "bit");
            await AssertColumnDefinition("tag_produce_info", "unicast_permitted", "bit");
            await AssertColumnDefinition("tag_produce_info", "maximum_rpi", "float");
            await AssertColumnDefinition("tag_produce_info", "minimum_rpi", "float");
            await AssertColumnDefinition("tag_produce_info", "default_rpi", "float");
            await AssertColumnDefinition("tag_produce_info", "record_hash", "nvarchar");

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
            await AssertColumnDefinition("tag_consume_info", "snapshot_id", "int");
            await AssertColumnDefinition("tag_consume_info", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_consume_info", "producer", "nvarchar");
            await AssertColumnDefinition("tag_consume_info", "remote_tag", "nvarchar");
            await AssertColumnDefinition("tag_consume_info", "remote_instance", "int");
            await AssertColumnDefinition("tag_consume_info", "rpi", "float");
            await AssertColumnDefinition("tag_consume_info", "unicast", "bit");
            await AssertColumnDefinition("tag_consume_info", "record_hash", "nvarchar");

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
            await AssertColumnDefinition("tag_alias", "snapshot_id", "int");
            await AssertColumnDefinition("tag_alias", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_alias", "alias_for", "nvarchar");

            await AssertPrimaryKey("tag_alias", "alias_id");
            await AssertForeignKey("tag_alias", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("tag_alias", "snapshot_id", "tag_id");
        }
    }
}
