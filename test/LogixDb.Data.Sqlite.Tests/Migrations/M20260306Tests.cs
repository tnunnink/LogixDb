namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260306Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202603061100_CreatesRungTableWithExpectedColumns()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("rung");

            await AssertColumnDefinition("rung", "rung_id", "integer");
            await AssertColumnDefinition("rung", "container_name", "text");
            await AssertColumnDefinition("rung", "routine_name", "text");
            await AssertColumnDefinition("rung", "rung_number", "integer");
            await AssertColumnDefinition("rung", "rung_text", "text");
            await AssertColumnDefinition("rung", "rung_comment", "text");
            await AssertColumnDefinition("rung", "code_hash", "text");
            await AssertColumnDefinition("rung", "record_hash", "text");

            await AssertPrimaryKey("rung", "rung_id");
            await AssertUniqueIndex("rung", "record_hash");
            await AssertIndex("rung", "container_name", "routine_name", "rung_number");
            await AssertIndex("rung", "routine_name", "rung_number");
            await AssertIndex("rung", "code_hash");
        }
    }


    [Test]
    public async Task MigrateUp_ToM202603061200_CreatesInstructionTableWithExpectedColumns()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("rung_instruction");

            await AssertColumnDefinition("rung_instruction", "rung_id", "integer");
            await AssertColumnDefinition("rung_instruction", "instruction_index", "integer");
            await AssertColumnDefinition("rung_instruction", "instruction_key", "text");
            await AssertColumnDefinition("rung_instruction", "instruction_text", "text");
            await AssertColumnDefinition("rung_instruction", "is_conditional", "integer");
            await AssertColumnDefinition("rung_instruction", "is_native", "integer");
            await AssertColumnDefinition("rung_instruction", "record_hash", "text");

            await AssertForeignKey("rung_instruction", "rung_id", "rung", "rung_id");
            await AssertUniqueIndex("rung_instruction", "rung_id", "instruction_index");
            await AssertIndex("rung_instruction", "instruction_key");
            await AssertIndex("rung_instruction", "record_hash");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202603061300_CreatesArgumentTableWithExpectedColumns()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("rung_argument");

            await AssertColumnDefinition("rung_argument", "rung_id", "integer");
            await AssertColumnDefinition("rung_argument", "instruction_index", "integer");
            await AssertColumnDefinition("rung_argument", "argument_index", "integer");
            await AssertColumnDefinition("rung_argument", "argument_type", "text");
            await AssertColumnDefinition("rung_argument", "argument_text", "text");

            await AssertForeignKey("rung_argument", "rung_id", "rung", "rung_id");
            await AssertUniqueIndex("rung_argument", "rung_id", "instruction_index", "argument_index");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202603061400_CreatesRungReferenceTableWithExpectedColumns()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("rung_reference");

            await AssertColumnDefinition("rung_reference", "rung_id", "integer");
            await AssertColumnDefinition("rung_reference", "instruction_index", "integer");
            await AssertColumnDefinition("rung_reference", "argument_index", "integer");
            await AssertColumnDefinition("rung_reference", "reference_name", "text");

            await AssertForeignKey("rung_reference", "rung_id", "rung", "rung_id");
            await AssertIndex("rung_reference", "rung_id", "instruction_index", "argument_index");
            await AssertIndex("rung_reference", "reference_name", "rung_id", "instruction_index", "argument_index");
        }
    }
}