namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260611Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202606112100_CreatesModuleConnectionTableWithExpectedColumns()
    {
        await Database.Migrate(202606112100);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("module_connection");

            await AssertColumnDefinition("module_connection", "connection_id", "integer");
            await AssertColumnDefinition("module_connection", "module_id", "integer");
            await AssertColumnDefinition("module_connection", "connection_name", "text");
            await AssertColumnDefinition("module_connection", "rpi", "integer");
            await AssertColumnDefinition("module_connection", "connection_type", "text");
            await AssertColumnDefinition("module_connection", "connection_priority", "text");
            await AssertColumnDefinition("module_connection", "transmission_type", "text");
            await AssertColumnDefinition("module_connection", "production_trigger", "text");
            await AssertColumnDefinition("module_connection", "output_redundant_owner", "integer");
            await AssertColumnDefinition("module_connection", "unicast", "integer");
            await AssertColumnDefinition("module_connection", "programatically_send_event_trigger", "integer");
            await AssertColumnDefinition("module_connection", "event_id", "integer");
            await AssertColumnDefinition("module_connection", "input_tag", "text");
            await AssertColumnDefinition("module_connection", "input_size", "integer");
            await AssertColumnDefinition("module_connection", "input_suffix", "text");
            await AssertColumnDefinition("module_connection", "output_tag", "text");
            await AssertColumnDefinition("module_connection", "output_size", "integer");
            await AssertColumnDefinition("module_connection", "output_suffix", "text");
            await AssertColumnDefinition("module_connection", "connection_path", "text");
            await AssertColumnDefinition("module_connection", "record_hash", "text");

            await AssertPrimaryKey("module_connection", "connection_id");
            await AssertUniqueIndex("module_connection", "module_id", "record_hash");
        }
    }
}