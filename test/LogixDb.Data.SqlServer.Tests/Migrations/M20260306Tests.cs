namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260306Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM014_CreatesInstructionTableWithExpectedColumns()
    {
        await Database.Migrate(202603061200);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("instruction");

            await AssertColumnDefinition("instruction", "instruction_id", "int");
            await AssertColumnDefinition("instruction", "snapshot_id", "int");
            await AssertColumnDefinition("instruction", "parent_hash", "nvarchar");
            await AssertColumnDefinition("instruction", "instruction_key", "nvarchar");
            await AssertColumnDefinition("instruction", "instruction_text", "nvarchar");
            await AssertColumnDefinition("instruction", "is_destructive", "bit");
            await AssertColumnDefinition("instruction", "is_native", "bit");

            await AssertPrimaryKey("instruction", "instruction_id");
            await AssertForeignKey("instruction", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("instruction", "snapshot_id", "parent_hash");
        }
    }

    [Test]
    public async Task MigrateUp_ToM015_CreatesArgumentTableWithExpectedColumns()
    {
        await Database.Migrate(202603061300);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("argument");

            await AssertColumnDefinition("argument", "argument_id", "int");
            await AssertColumnDefinition("argument", "snapshot_id", "int");
            await AssertColumnDefinition("argument", "parent_hash", "nvarchar");
            await AssertColumnDefinition("argument", "argument_type", "nvarchar");
            await AssertColumnDefinition("argument", "argument_text", "nvarchar");
            await AssertColumnDefinition("argument", "argument_tags", "nvarchar");
            await AssertColumnDefinition("argument", "argument_values", "nvarchar");

            await AssertPrimaryKey("argument", "argument_id");
            await AssertForeignKey("argument", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("argument", "snapshot_id", "parent_hash");
        }
    }
}
