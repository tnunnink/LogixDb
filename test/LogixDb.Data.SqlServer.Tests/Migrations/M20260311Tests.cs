namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260311Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM019_CreatesTagCommentTableWithExpectedColumns()
    {
        await Database.Migrate(202603110830);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("tag_comment");

            await AssertColumnDefinition("tag_comment", "comment_id", "int");
            await AssertColumnDefinition("tag_comment", "snapshot_id", "int");
            await AssertColumnDefinition("tag_comment", "program_name", "nvarchar");
            await AssertColumnDefinition("tag_comment", "tag_name", "nvarchar");
            await AssertColumnDefinition("tag_comment", "tag_comment", "nvarchar");

            await AssertPrimaryKey("tag_comment", "comment_id");
            await AssertForeignKey("tag_comment", "snapshot_id", "snapshot", "snapshot_id");
            await AssertIndex("tag_comment", "snapshot_id", "program_name", "tag_name");
            await AssertIndex("tag_comment", "tag_name", "snapshot_id");
        }
    }
}
