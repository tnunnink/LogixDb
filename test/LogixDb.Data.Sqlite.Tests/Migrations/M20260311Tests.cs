namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260311Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM019_CreatesTagCommentTableWithExpectedColumns()
    {
        await Database.Migrate(202603110830);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_comment");

            await AssertColumnDefinition("tag_comment", "comment_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_comment", "snapshot_id", "integer");
            await AssertColumnDefinition("tag_comment", "tag_id", "uniqueidentifier");
            await AssertColumnDefinition("tag_comment", "tag_name", "text");
            await AssertColumnDefinition("tag_comment", "tag_comment", "text");

            await AssertPrimaryKey("tag_comment", "comment_id");
            await AssertForeignKey("tag_comment", "snapshot_id", "snapshot", "snapshot_id");
            await AssertIndex("tag_comment", "tag_name", "snapshot_id");
        }
    }
}
