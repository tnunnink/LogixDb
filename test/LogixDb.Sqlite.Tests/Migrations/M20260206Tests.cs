namespace LogixDb.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260206Tests : SqliteMigrationTestBase
{
    [Test]
    public void MigrateUp_ToM001_CreatesTargetTableWithExpectedColumns()
    {
        MigrateUp(toVersion: 202602061010);

        using var connection = OpenConnection();

        using (Assert.EnterMultipleScope())
        {
            AssertTableExists(connection, "target");

            AssertColumnDefinition(connection, "target", "target_id", "integer");
            AssertColumnDefinition(connection, "target", "target_key", "text");
            AssertColumnDefinition(connection, "target", "created_on", "datetime");

            AssertPrimaryKey(connection, "target", "target_id");
            AssertUniqueIndex(connection, "target", "target_key");
        }
    }

    [Test]
    public void MigrateUp_ToM002_CreatesTargetTableWithExpectedColumns()
    {
        MigrateUp(toVersion: 202602061020);

        using var connection = OpenConnection();

        using (Assert.EnterMultipleScope())
        {
            AssertTableExists(connection, "snapshot");

            AssertColumnDefinition(connection, "snapshot", "snapshot_id", "integer");
            AssertColumnDefinition(connection, "snapshot", "target_id", "integer");
            AssertColumnDefinition(connection, "snapshot", "target_type", "text");
            AssertColumnDefinition(connection, "snapshot", "target_name", "text");
            AssertColumnDefinition(connection, "snapshot", "is_partial", "integer");
            AssertColumnDefinition(connection, "snapshot", "schema_revision", "text");
            AssertColumnDefinition(connection, "snapshot", "software_revision", "text");
            AssertColumnDefinition(connection, "snapshot", "export_date", "datetime");
            AssertColumnDefinition(connection, "snapshot", "export_options", "text");
            AssertColumnDefinition(connection, "snapshot", "import_date", "datetime");
            AssertColumnDefinition(connection, "snapshot", "import_user", "text");
            AssertColumnDefinition(connection, "snapshot", "import_machine", "text");
            AssertColumnDefinition(connection, "snapshot", "source_hash", "text");
            AssertColumnDefinition(connection, "snapshot", "source_data", "blob");

            AssertPrimaryKey(connection, "snapshot", "snapshot_id");
            AssertForeignKey(connection, "snapshot", "target_id", "target", "target_id");
        }
    }
}