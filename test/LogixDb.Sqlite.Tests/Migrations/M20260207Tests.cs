namespace LogixDb.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260207Tests() : SqliteMigrationTestBase(typeof(LogixSqliteDatabase).Assembly)
{
    [Test]
    public void MigrateUp_ToM01_CreatesTagTableWithExpectedColumns()
    {
        MigrateUp(toVersion: 202602070830);

        using var connection = OpenConnection();

        using (Assert.EnterMultipleScope())
        {
            AssertTableExists(connection, "tag");

            AssertColumn(connection, "tag", "tag_id", "integer");
            AssertColumn(connection, "tag", "snapshot_id", "integer");
            AssertColumn(connection, "tag", "reference", "text");
            AssertColumn(connection, "tag", "base_name", "text");
            AssertColumn(connection, "tag", "tag_name", "text");
            AssertColumn(connection, "tag", "depth", "integer");
            AssertColumn(connection, "tag", "scope", "text");
            AssertColumn(connection, "tag", "usage", "text");
            AssertColumn(connection, "tag", "data_type", "text");
            AssertColumn(connection, "tag", "value", "text");
            AssertColumn(connection, "tag", "description", "text");
            AssertColumn(connection, "tag", "dimensions", "text");
            AssertColumn(connection, "tag", "radix", "text");
            AssertColumn(connection, "tag", "external_access", "text");
            AssertColumn(connection, "tag", "opcua_access", "text");
            AssertColumn(connection, "tag", "constant", "integer");
            AssertColumn(connection, "tag", "tag_type", "text");
            AssertColumn(connection, "tag", "alias_for", "text");
            AssertColumn(connection, "tag", "component_class", "text");

            AssertPrimaryKey(connection, "tag", "tag_id");
            AssertForeignKey(connection, "tag", "snapshot_id", "snapshot", "snapshot_id");
            AssertUniqueIndex(connection, "tag", "snapshot_id", "reference");
        }
    }
}