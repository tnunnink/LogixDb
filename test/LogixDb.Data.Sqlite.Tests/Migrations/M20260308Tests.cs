namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260308Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM016_CreatesOperandTableWithExpectedColumns()
    {
        await Database.Migrate(202603082100);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("operand");

            await AssertColumnDefinition("operand", "operand_id", "integer");
            await AssertColumnDefinition("operand", "snapshot_id", "integer");
            await AssertColumnDefinition("operand", "instruction_key", "text");
            await AssertColumnDefinition("operand", "operand_index", "integer");
            await AssertColumnDefinition("operand", "operand_name", "text");
            await AssertColumnDefinition("operand", "operand_type", "text");
            await AssertColumnDefinition("operand", "operand_format", "text");
            await AssertColumnDefinition("operand", "operand_description", "text");
            await AssertColumnDefinition("operand", "is_destructive", "integer");

            await AssertPrimaryKey("operand", "operand_id");
            await AssertForeignKey("operand", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("operand", "snapshot_id", "instruction_key", "operand_index");
        }
    }

    [Test]
    public async Task MigrateUp_ToM017_SeedsExpectedOperands()
    {
        await Database.Migrate(202603082130);

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