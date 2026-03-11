namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260309Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM018_CreatesSnapshotInfoTableWithExpectedColumns()
    {
        await Database.Migrate(202603092030);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("snapshot_info");

            await AssertColumnDefinition("snapshot_info", "snapshot_id", "int");
            await AssertColumnDefinition("snapshot_info", "key", "nvarchar");
            await AssertColumnDefinition("snapshot_info", "value", "nvarchar");

            await AssertForeignKey("snapshot_info", "snapshot_id", "snapshot", "snapshot_id");
        }
    }
}
