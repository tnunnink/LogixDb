namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260312Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM020_CreatesAoiRungTableWithExpectedColumns()
    {
        await Database.Migrate(202603122030);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("aoi_rung");

            await AssertColumnDefinition("aoi_rung", "rung_id", "uniqueidentifier");
            await AssertColumnDefinition("aoi_rung", "snapshot_id", "int");
            await AssertColumnDefinition("aoi_rung", "aoi_name", "nvarchar");
            await AssertColumnDefinition("aoi_rung", "routine_name", "nvarchar");
            await AssertColumnDefinition("aoi_rung", "rung_number", "int");
            await AssertColumnDefinition("aoi_rung", "rung_text", "nvarchar");
            await AssertColumnDefinition("aoi_rung", "rung_comment", "nvarchar");
            await AssertColumnDefinition("aoi_rung", "record_hash", "nvarchar");

            await AssertPrimaryKey("aoi_rung", "rung_id");
            await AssertForeignKey("aoi_rung", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("aoi_rung", "snapshot_id", "aoi_name", "routine_name", "rung_number");
        }
    }
}
