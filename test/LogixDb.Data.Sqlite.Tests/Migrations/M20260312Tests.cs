namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260312Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM020_CreatesAoiRungTableWithExpectedColumns()
    {
        await Database.Migrate(202603122030);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("aoi_rung");

            await AssertColumnDefinition("aoi_rung", "rung_id", "integer");
            await AssertColumnDefinition("aoi_rung", "snapshot_id", "integer");
            await AssertColumnDefinition("aoi_rung", "aoi_name", "text");
            await AssertColumnDefinition("aoi_rung", "routine_name", "text");
            await AssertColumnDefinition("aoi_rung", "rung_number", "integer");
            await AssertColumnDefinition("aoi_rung", "rung_text", "text");
            await AssertColumnDefinition("aoi_rung", "rung_comment", "text");
            await AssertColumnDefinition("aoi_rung", "record_hash", "text");

            await AssertPrimaryKey("aoi_rung", "rung_id");
            await AssertForeignKey("aoi_rung", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("aoi_rung", "snapshot_id", "aoi_name", "routine_name", "rung_number");
        }
    }
}
