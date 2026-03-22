namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260207Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM003_CreatesTagTableWithExpectedColumns()
    {
        await Database.Migrate(202602070830);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag");

            await AssertColumnDefinition("tag", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag", "snapshot_id", "int");
            await AssertColumnDefinition("tag", "program_name", "nvarchar");
            await AssertColumnDefinition("tag", "tag_name", "nvarchar");
            await AssertColumnDefinition("tag", "base_name", "nvarchar");
            await AssertColumnDefinition("tag", "parent_name", "nvarchar");
            await AssertColumnDefinition("tag", "member_name", "nvarchar");
            await AssertColumnDefinition("tag", "data_type", "nvarchar");
            await AssertColumnDefinition("tag", "tag_value", "nvarchar");
            await AssertColumnDefinition("tag", "tag_usage", "nvarchar");
            await AssertColumnDefinition("tag", "external_access", "nvarchar");
            await AssertColumnDefinition("tag", "is_constant", "bit");
            await AssertColumnDefinition("tag", "record_hash", "nvarchar");

            await AssertPrimaryKey("tag", "tag_id");
            await AssertForeignKey("tag", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("tag", "snapshot_id", "program_name", "tag_name");
            await AssertIndex("tag", "tag_name", "snapshot_id");
            await AssertIndex("tag", "data_type", "snapshot_id");
            await AssertIndex("tag", "base_name", "snapshot_id");
            await AssertIndex("tag", "snapshot_id", "parent_name");
            await AssertIndex("tag", "snapshot_id", "member_name");
            await AssertIndex("tag", "record_hash", "snapshot_id");
        }
    }
}