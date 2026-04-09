using Dapper;

namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260308Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202603082100_CreatesOperandTableWithExpectedColumns()
    {
        await Database.Migrate(202603082100);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("operand");
            
            await AssertColumnDefinition("operand", "operand_id", "uniqueidentifier");
            await AssertColumnDefinition("operand", "snapshot_id", "int");
            await AssertColumnDefinition("operand", "instruction_key", "nvarchar");
            await AssertColumnDefinition("operand", "operand_index", "tinyint");
            await AssertColumnDefinition("operand", "operand_name", "nvarchar");
            await AssertColumnDefinition("operand", "operand_type", "nvarchar");
            await AssertColumnDefinition("operand", "operand_format", "nvarchar");
            await AssertColumnDefinition("operand", "operand_description", "nvarchar");
            await AssertColumnDefinition("operand", "is_destructive", "bit");

            await AssertPrimaryKey("operand", "operand_id");
            await AssertForeignKey("operand", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("operand", "snapshot_id", "instruction_key", "operand_index");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202603082130_SeedsExpectedOperands()
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
