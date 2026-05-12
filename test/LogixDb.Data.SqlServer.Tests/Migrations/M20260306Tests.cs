namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260306Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202603061200_CreatesInstructionTableWithExpectedColumns()
    {
        await Database.Migrate(202603061200);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("instruction");

            await AssertColumnDefinition("instruction", "rung_key", "uniqueidentifier");
            await AssertColumnDefinition("instruction", "instruction_index", "smallint");
            await AssertColumnDefinition("instruction", "instruction_key", "nvarchar");
            await AssertColumnDefinition("instruction", "instruction_text", "nvarchar");
            await AssertColumnDefinition("instruction", "is_conditional", "bit");
            await AssertColumnDefinition("instruction", "is_native", "bit");
            await AssertColumnDefinition("instruction", "record_hash", "nvarchar");

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
            await AssertColumnDefinition("argument", "instruction_index", "smallint");
            await AssertColumnDefinition("argument", "argument_index", "tinyint");
            await AssertColumnDefinition("argument", "argument_type", "nvarchar");
            await AssertColumnDefinition("argument", "argument_text", "nvarchar");

            await AssertForeignKey("argument", "rung_key", "rung", "rung_key");
            await AssertIndex("argument", "rung_key", "instruction_index", "argument_index");
            await AssertIndex("argument", "argument_text");
        }
    }
}
