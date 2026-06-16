namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260527Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202605271009_CreatesCoreVersionedFunctions()
    {
        await Database.Migrate(202605271009);

        using (Assert.EnterMultipleScope())
        {
            await AssertFunctionExists("controllers_at_version");
            await AssertFunctionExists("data_types_at_version");
            await AssertFunctionExists("aois_at_version");
            await AssertFunctionExists("modules_at_version");
            await AssertFunctionExists("tags_at_version");
            await AssertFunctionExists("tasks_at_version");
            await AssertFunctionExists("programs_at_version");
            await AssertFunctionExists("routines_at_version");
            await AssertFunctionExists("rungs_at_version");
        }
    }
    
    [Test]
    public async Task MigrateUp_ToM202605271010_CreatesCoreVersionedFunctions()
    {
        await Database.Migrate(202605271010);

        using (Assert.EnterMultipleScope())
        {
            await AssertFunctionExists("logic_at_version");
        }
    }
}
