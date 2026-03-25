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
            await AssertTableExists("tag_producer");

            await AssertColumnDefinition("tag_producer", "producer_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_producer", "snapshot_id", "int");
            await AssertColumnDefinition("tag_producer", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_producer", "produce_count", "int");
            await AssertColumnDefinition("tag_producer", "programatically_send_event_trigger", "bit");
            await AssertColumnDefinition("tag_producer", "unicast_permitted", "bit");
            await AssertColumnDefinition("tag_producer", "maximum_rpi", "float");
            await AssertColumnDefinition("tag_producer", "minimum_rpi", "float");
            await AssertColumnDefinition("tag_producer", "default_rpi", "float");
            await AssertColumnDefinition("tag_producer", "record_hash", "nvarchar");

            await AssertPrimaryKey("tag_producer", "producer_id");
            await AssertForeignKey("tag_producer", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("tag_producer", "snapshot_id", "tag_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM022_CreatesTagConsumeInfoTableWithExpectedColumns()
    {
        await Database.Migrate(202603220530);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_consumer");

            await AssertColumnDefinition("tag_consumer", "consumer_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_consumer", "snapshot_id", "int");
            await AssertColumnDefinition("tag_consumer", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_consumer", "producer", "nvarchar");
            await AssertColumnDefinition("tag_consumer", "remote_tag", "nvarchar");
            await AssertColumnDefinition("tag_consumer", "remote_instance", "int");
            await AssertColumnDefinition("tag_consumer", "rpi", "float");
            await AssertColumnDefinition("tag_consumer", "unicast", "bit");
            await AssertColumnDefinition("tag_consumer", "record_hash", "nvarchar");

            await AssertPrimaryKey("tag_consumer", "consumer_id");
            await AssertForeignKey("tag_consumer", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("tag_consumer", "snapshot_id", "tag_id");
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
