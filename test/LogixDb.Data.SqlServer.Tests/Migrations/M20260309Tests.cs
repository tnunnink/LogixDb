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
            await AssertTableExists("snapshot_property");

            await AssertColumnDefinition("snapshot_property", "property_id", "int");
            await AssertColumnDefinition("snapshot_property", "snapshot_id", "int");
            await AssertColumnDefinition("snapshot_property", "property_name", "nvarchar");
            await AssertColumnDefinition("snapshot_property", "property_value", "nvarchar");

            await AssertPrimaryKey("snapshot_property", "property_id");
            await AssertForeignKey("snapshot_property", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("snapshot_property", "snapshot_id", "property_name");
        }
    }
}
