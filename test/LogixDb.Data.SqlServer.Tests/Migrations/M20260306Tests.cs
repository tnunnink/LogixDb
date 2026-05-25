namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260306Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202603061100_CreatesRungTableWithExpectedColumns()
    {
        await Database.Migrate(202603061100);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("rung");
            
            await AssertColumnDefinition("rung", "rung_id", "uniqueidentifier");
            await AssertColumnDefinition("rung", "routine_id", "bigint");
            await AssertColumnDefinition("rung", "rung_number", "int");
            await AssertColumnDefinition("rung", "rung_text", "nvarchar");
            await AssertColumnDefinition("rung", "rung_comment", "nvarchar");
            await AssertColumnDefinition("rung", "code_hash", "nvarchar");
            await AssertColumnDefinition("rung", "record_hash", "nvarchar");
            
            await AssertPrimaryKey("rung", "rung_id");
            await AssertUniqueIndex("rung", "record_hash");
            await AssertUniqueIndex("rung", "routine_id", "rung_number");
            await AssertIndex("rung", "code_hash");
        }
    }
    
    [Test]
    public async Task MigrateUp_ToM202603061200_CreatesInstructionTableWithExpectedColumns()
    {
        await Database.Migrate(202603061200);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("rung_instruction");

            await AssertColumnDefinition("rung_instruction", "rung_id", "uniqueidentifier");
            await AssertColumnDefinition("rung_instruction", "instruction_index", "smallint");
            await AssertColumnDefinition("rung_instruction", "instruction_key", "nvarchar");
            await AssertColumnDefinition("rung_instruction", "instruction_text", "nvarchar");
            await AssertColumnDefinition("rung_instruction", "is_conditional", "bit");
            await AssertColumnDefinition("rung_instruction", "is_native", "bit");
            await AssertColumnDefinition("rung_instruction", "record_hash", "nvarchar");

            await AssertForeignKey("rung_instruction", "rung_id", "rung", "rung_id");
            await AssertUniqueIndex("rung_instruction", "rung_id", "instruction_index");
            await AssertIndex("rung_instruction", "instruction_key");
            await AssertIndex("rung_instruction", "record_hash");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202603061300_CreatesArgumentTableWithExpectedColumns()
    {
        await Database.Migrate(202603061300);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("rung_argument");

            await AssertColumnDefinition("rung_argument", "rung_id", "uniqueidentifier");
            await AssertColumnDefinition("rung_argument", "instruction_index", "smallint");
            await AssertColumnDefinition("rung_argument", "argument_index", "tinyint");
            await AssertColumnDefinition("rung_argument", "argument_type", "nvarchar");
            await AssertColumnDefinition("rung_argument", "argument_text", "nvarchar");

            await AssertForeignKey("rung_argument", "rung_id", "rung", "rung_id");
            await AssertIndex("rung_argument", "rung_id", "instruction_index", "argument_index");
            await AssertIndex("rung_argument", "argument_text");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202603061400_CreatesRungReferenceTableWithExpectedColumns()
    {
        await Database.Migrate(202603061400);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("rung_reference");

            await AssertColumnDefinition("rung_reference", "rung_id", "uniqueidentifier");
            await AssertColumnDefinition("rung_reference", "instruction_index", "smallint");
            await AssertColumnDefinition("rung_reference", "argument_index", "tinyint");
            await AssertColumnDefinition("rung_reference", "reference_name", "nvarchar");

            await AssertForeignKey("rung_reference", "rung_id", "rung", "rung_id");
            await AssertIndex("rung_reference", "rung_id", "instruction_index", "argument_index");
            await AssertIndex("rung_reference", "reference_name", "rung_id", "instruction_index", "argument_index");
        }
    }
}
