namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260616Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202606160820_CreatesTypeTreeAtVersionFunction()
    {
        await Database.Migrate(202606160820);

        await AssertFunctionExists("type_tree_at_version");
    }

    [Test]
    public async Task MigrateUp_ToM202606161358_CreatesGetLatestVersionIdFunction()
    {
        await Database.Migrate(202606161358);

        await AssertFunctionExists("get_latest_version_id");
    }
}
