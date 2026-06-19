namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260308Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202603082100_CreatesOperandTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("operand");

            await AssertColumnDefinition("operand", "operand_id", "integer");
            await AssertColumnDefinition("operand", "instruction_key", "text");
            await AssertColumnDefinition("operand", "operand_index", "integer");
            await AssertColumnDefinition("operand", "operand_name", "text");
            await AssertColumnDefinition("operand", "operand_type", "text");
            /*await AssertColumnDefinition("operand", "operand_format", "text");*/
            await AssertColumnDefinition("operand", "operand_description", "text");
            await AssertColumnDefinition("operand", "is_destructive", "integer");
            await AssertColumnDefinition("operand", "is_native", "integer");
            await AssertColumnDefinition("operand", "record_hash", "text");

            await AssertPrimaryKey("operand", "operand_id");
            await AssertUniqueIndex("operand", "record_hash");
            await AssertIndex("operand", "instruction_key", "operand_index");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202603082130_SeedsExpectedOperands()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertRecordExists("operand", "instruction_key", "ABS");
            await AssertRecordExists("operand", "instruction_key", "ALMA");
            await AssertRecordExists("operand", "instruction_key", "MOVE");
            await AssertRecordExists("operand", "instruction_key", "MOV");
            await AssertRecordExists("operand", "instruction_key", "OTE");
            await AssertRecordExists("operand", "instruction_key", "XIC");
            await AssertRecordExists("operand", "instruction_key", "TON");
            await AssertRecordExists("operand", "operand_name", "source");
            await AssertRecordExists("operand", "operand_name", "destination");
        }
    }
}