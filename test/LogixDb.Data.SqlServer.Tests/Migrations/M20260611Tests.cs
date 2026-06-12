namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260611Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202606111410_CreatesTagTreeFromFunction()
    {
        await Database.Migrate(202606111410);

        await AssertFunctionExists("tag_tree_from");
    }
}
