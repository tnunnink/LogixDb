namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260309Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM018_CreatesSnapshotInfoTableWithExpectedColumns()
    {
        await Database.Migrate(202603092030);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("snapshot_property");

            await AssertColumnDefinition("snapshot_property", "property_id", "uniqueidentifier");
            await AssertColumnDefinition("snapshot_property", "snapshot_id", "integer");
            await AssertColumnDefinition("snapshot_property", "property_name", "text");
            await AssertColumnDefinition("snapshot_property", "property_value", "text");

            await AssertPrimaryKey("snapshot_property", "property_id");
            await AssertForeignKey("snapshot_property", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("snapshot_property", "snapshot_id", "property_name");
        }
    }
}
