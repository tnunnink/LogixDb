namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260306Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM014_CreatesInstructionTableWithExpectedColumns()
    {
        await Database.Migrate(202603061200);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("instruction");

            await AssertColumnDefinition("instruction", "instruction_id", "uniqueidentifier");
            await AssertColumnDefinition("instruction", "snapshot_id", "integer");
            await AssertColumnDefinition("instruction", "rung_id", "uniqueidentifier");
            await AssertColumnDefinition("instruction", "instruction_index", "integer");
            await AssertColumnDefinition("instruction", "instruction_key", "text");
            await AssertColumnDefinition("instruction", "instruction_text", "text");
            await AssertColumnDefinition("instruction", "is_conditional", "integer");
            await AssertColumnDefinition("instruction", "is_native", "integer");
            await AssertColumnDefinition("instruction", "record_hash", "text");

            await AssertPrimaryKey("instruction", "instruction_id");
            await AssertForeignKey("instruction", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("instruction", "snapshot_id", "rung_id", "instruction_index");
            await AssertIndex("instruction", "record_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM015_CreatesArgumentTableWithExpectedColumns()
    {
        await Database.Migrate(202603061300);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("argument");

            await AssertColumnDefinition("argument", "argument_id", "uniqueidentifier");
            await AssertColumnDefinition("argument", "snapshot_id", "integer");
            await AssertColumnDefinition("argument", "instruction_id", "uniqueidentifier");
            await AssertColumnDefinition("argument", "argument_index", "integer");
            await AssertColumnDefinition("argument", "argument_type", "text");
            await AssertColumnDefinition("argument", "argument_text", "text");

            await AssertPrimaryKey("argument", "argument_id");
            await AssertForeignKey("argument", "snapshot_id", "snapshot", "snapshot_id");
            await AssertIndex("argument", "snapshot_id", "instruction_id", "argument_index");
            await AssertIndex("argument", "snapshot_id", "argument_text");
        }
    }
}
