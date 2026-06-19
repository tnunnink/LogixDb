using Dapper;

namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260308Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202603082100_CreatesOperandTableWithExpectedColumns()
    {
        await Migrator.Migrate(Connection);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "operand");

            await AssertColumnDefinition("operand", "operand_id", "bigint");
            await AssertColumnDefinition("operand", "instruction_key", "nvarchar");
            await AssertColumnDefinition("operand", "operand_index", "tinyint");
            await AssertColumnDefinition("operand", "operand_name", "nvarchar");
            await AssertColumnDefinition("operand", "operand_type", "nvarchar");
            /*await AssertColumnDefinition("operand", "operand_format", "nvarchar");*/
            await AssertColumnDefinition("operand", "operand_description", "nvarchar");
            await AssertColumnDefinition("operand", "is_destructive", "bit");
            await AssertColumnDefinition("operand", "is_native", "bit");
            await AssertColumnDefinition("operand", "record_hash", "nvarchar");

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
            await AssertTableExists("logix", "operand");
            
            await AssertRecordExists("logix.operand", "instruction_key", "ABS");
            await AssertRecordExists("logix.operand", "instruction_key", "ALMA");
            await AssertRecordExists("logix.operand", "instruction_key", "MOVE");
            await AssertRecordExists("logix.operand", "instruction_key", "MOV");
            await AssertRecordExists("logix.operand", "instruction_key", "OTE");
            await AssertRecordExists("logix.operand", "instruction_key", "XIC");
            await AssertRecordExists("logix.operand", "instruction_key", "TON");
            await AssertRecordExists("logix.operand", "operand_name", "source");
            await AssertRecordExists("logix.operand", "operand_name", "destination");
        }
    }
}