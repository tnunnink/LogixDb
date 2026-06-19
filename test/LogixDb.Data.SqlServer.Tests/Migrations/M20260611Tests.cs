namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260611Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202606112100_CreatesModuleConnectionTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "module_connection");

            await AssertColumnDefinition("module_connection", "connection_id", "bigint");
            await AssertColumnDefinition("module_connection", "module_id", "bigint");
            await AssertColumnDefinition("module_connection", "connection_name", "nvarchar");
            await AssertColumnDefinition("module_connection", "rpi", "int");
            await AssertColumnDefinition("module_connection", "connection_type", "nvarchar");
            await AssertColumnDefinition("module_connection", "connection_priority", "nvarchar");
            await AssertColumnDefinition("module_connection", "transmission_type", "nvarchar");
            await AssertColumnDefinition("module_connection", "production_trigger", "nvarchar");
            await AssertColumnDefinition("module_connection", "output_redundant_owner", "bit");
            await AssertColumnDefinition("module_connection", "unicast", "bit");
            await AssertColumnDefinition("module_connection", "programatically_send_event_trigger", "bit");
            await AssertColumnDefinition("module_connection", "event_id", "int");
            await AssertColumnDefinition("module_connection", "input_tag", "nvarchar");
            await AssertColumnDefinition("module_connection", "input_size", "int");
            await AssertColumnDefinition("module_connection", "input_suffix", "nvarchar");
            await AssertColumnDefinition("module_connection", "output_tag", "nvarchar");
            await AssertColumnDefinition("module_connection", "output_size", "int");
            await AssertColumnDefinition("module_connection", "output_suffix", "nvarchar");
            await AssertColumnDefinition("module_connection", "connection_path", "nvarchar");
            await AssertColumnDefinition("module_connection", "record_hash", "nvarchar");

            await AssertPrimaryKey("module_connection", "connection_id");
            await AssertUniqueIndex("module_connection", "module_id", "record_hash");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202606112200_CreatesModulePortTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "module_port");

            await AssertColumnDefinition("module_port", "port_id", "bigint");
            await AssertColumnDefinition("module_port", "module_id", "bigint");
            await AssertColumnDefinition("module_port", "port_number", "smallint");
            await AssertColumnDefinition("module_port", "port_type", "nvarchar");
            await AssertColumnDefinition("module_port", "address", "nvarchar");
            await AssertColumnDefinition("module_port", "upstream", "bit");
            await AssertColumnDefinition("module_port", "bus_size", "tinyint");
            await AssertColumnDefinition("module_port", "record_hash", "nvarchar");

            await AssertPrimaryKey("module_port", "port_id");
            await AssertUniqueIndex("module_port", "module_id", "record_hash");
            await AssertForeignKey("module_port", "module_id", "module", "module_id");
        }
    }
}