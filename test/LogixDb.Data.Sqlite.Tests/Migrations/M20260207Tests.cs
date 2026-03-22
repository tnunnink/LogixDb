namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260207Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM003_CreatesTagTableWithExpectedColumns()
    {
        await Database.Migrate(202602070830);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag");

            await AssertColumnDefinition("tag", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag", "snapshot_id", "integer");
            await AssertColumnDefinition("tag", "program_name", "text");
            await AssertColumnDefinition("tag", "tag_name", "text");
            await AssertColumnDefinition("tag", "base_name", "text");
            await AssertColumnDefinition("tag", "parent_name", "text");
            await AssertColumnDefinition("tag", "member_name", "text");
            await AssertColumnDefinition("tag", "data_type", "text");
            await AssertColumnDefinition("tag", "tag_value", "text");
            await AssertColumnDefinition("tag", "tag_usage", "text");
            await AssertColumnDefinition("tag", "external_access", "text");
            await AssertColumnDefinition("tag", "is_constant", "integer");
            await AssertColumnDefinition("tag", "record_hash", "text");

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