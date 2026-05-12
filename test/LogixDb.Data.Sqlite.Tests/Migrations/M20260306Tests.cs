namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260306Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202603061200_CreatesInstructionTableWithExpectedColumns()
    {
        await Database.Migrate(202603061200);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("instruction");

            await AssertColumnDefinition("instruction", "rung_key", "uniqueidentifier");
            await AssertColumnDefinition("instruction", "instruction_index", "integer");
            await AssertColumnDefinition("instruction", "instruction_key", "text");
            await AssertColumnDefinition("instruction", "instruction_text", "text");
            await AssertColumnDefinition("instruction", "is_conditional", "integer");
            await AssertColumnDefinition("instruction", "is_native", "integer");
            await AssertColumnDefinition("instruction", "record_hash", "text");

            await AssertForeignKey("instruction", "rung_key", "rung", "rung_key");
            await AssertUniqueIndex("instruction", "rung_key", "instruction_index");
            await AssertIndex("instruction", "instruction_key");
            await AssertIndex("instruction", "record_hash");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202603061300_CreatesArgumentTableWithExpectedColumns()
    {
        await Database.Migrate(202603061300);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("argument");

            await AssertColumnDefinition("argument", "rung_key", "uniqueidentifier");
            await AssertColumnDefinition("argument", "instruction_index", "integer");
            await AssertColumnDefinition("argument", "argument_index", "integer");
            await AssertColumnDefinition("argument", "argument_type", "text");
            await AssertColumnDefinition("argument", "argument_text", "text");

            await AssertForeignKey("argument", "rung_key", "rung", "rung_key");
            await AssertIndex("argument", "rung_key", "instruction_index", "argument_index");
            await AssertIndex("argument", "argument_text");
        }
    }
}