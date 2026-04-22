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

            await AssertColumnDefinition("instruction", "instruction_id", "uniqueidentifier");
            await AssertColumnDefinition("instruction", "instance_id", "int");
            await AssertColumnDefinition("instruction", "rung_id", "uniqueidentifier");
            await AssertColumnDefinition("instruction", "instruction_index", "smallint");
            await AssertColumnDefinition("instruction", "instruction_key", "nvarchar");
            await AssertColumnDefinition("instruction", "instruction_text", "nvarchar");
            await AssertColumnDefinition("instruction", "is_conditional", "bit");
            await AssertColumnDefinition("instruction", "is_native", "bit");
            await AssertColumnDefinition("instruction", "record_hash", "nvarchar");

            await AssertPrimaryKey("instruction", "instruction_id");
            await AssertForeignKey("instruction", "instance_id", "target_instance", "instance_id");
            await AssertForeignKey("instruction", "rung_id", "rung", "rung_id");
            await AssertUniqueIndex("instruction", "rung_id", "instruction_index");
            await AssertIndex("instruction", "record_hash", "rung_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202603061300_CreatesArgumentTableWithExpectedColumns()
    {
        await Database.Migrate(202603061300);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("argument");

            await AssertColumnDefinition("argument", "argument_id", "uniqueidentifier");
            await AssertColumnDefinition("argument", "instance_id", "int");
            await AssertColumnDefinition("argument", "instruction_id", "uniqueidentifier");
            await AssertColumnDefinition("argument", "argument_index", "tinyint");
            await AssertColumnDefinition("argument", "argument_type", "nvarchar");
            await AssertColumnDefinition("argument", "argument_text", "nvarchar");

            await AssertPrimaryKey("argument", "argument_id");
            await AssertForeignKey("argument", "instance_id", "target_instance", "instance_id");
            await AssertForeignKey("argument", "instruction_id", "instruction", "instruction_id");
            await AssertIndex("argument", "instruction_id", "argument_index");
        }
    }
}
