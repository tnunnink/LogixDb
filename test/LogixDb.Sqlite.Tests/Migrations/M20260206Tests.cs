namespace LogixDb.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260206Tests() : SqliteMigrationTestBase(typeof(LogixSqliteDatabase).Assembly)
{
    [Test]
    public void MigrateUp_ToM01_CreatesTargetTableWithExpectedColumns()
    {
        MigrateUp(toVersion: 202602061010);

        using var connection = OpenConnection();

        using (Assert.EnterMultipleScope())
        {
            AssertTableExists(connection, "target");

            AssertColumn(connection, "target", "target_id", "integer");
            AssertColumn(connection, "target", "target_key", "text");
            AssertColumn(connection, "target", "target_type", "text");
            AssertColumn(connection, "target", "target_name", "text");
            AssertColumn(connection, "target", "is_partial", "integer");
            AssertColumn(connection, "target", "created_on", "datetime");

            AssertPrimaryKey(connection, "target", "target_id");
            AssertUniqueIndex(connection, "target", "target_key");
        }
    }

    [Test]
    public void MigrateUp_ToM02_CreatesTargetTableWithExpectedColumns()
    {
        MigrateUp(toVersion: 202602061020);

        using var connection = OpenConnection();

        using (Assert.EnterMultipleScope())
        {
            AssertTableExists(connection, "snapshot");

            AssertColumn(connection, "snapshot", "snapshot_id", "integer");
            AssertColumn(connection, "snapshot", "target_id", "integer");
            AssertColumn(connection, "snapshot", "source_revision", "text");
            AssertColumn(connection, "snapshot", "source_hash", "text");
            AssertColumn(connection, "snapshot", "exported_on", "datetime");
            AssertColumn(connection, "snapshot", "imported_on", "datetime");
            AssertColumn(connection, "snapshot", "imported_by", "text");
            AssertColumn(connection, "snapshot", "imported_from", "text");
            AssertColumn(connection, "snapshot", "options", "text");

            AssertPrimaryKey(connection, "snapshot", "snapshot_id");
        }
    }
}