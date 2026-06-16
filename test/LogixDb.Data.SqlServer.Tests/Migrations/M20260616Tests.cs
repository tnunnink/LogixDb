namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260616Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202606160820_CreatesTypeTreeForFunction()
    {
        await Database.Migrate(202606160820);

        await AssertFunctionExists("type_tree_for");
    }
}
