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
            await AssertFunctionExists("controllers_for");
            await AssertFunctionExists("data_types_for");
            await AssertFunctionExists("aois_for");
            await AssertFunctionExists("modules_for");
            await AssertFunctionExists("tags_for");
            await AssertFunctionExists("tasks_for");
            await AssertFunctionExists("programs_for");
            await AssertFunctionExists("routines_for");
            await AssertFunctionExists("rungs_for");
        }
    }
    
    [Test]
    public async Task MigrateUp_ToM202605271010_CreatesCoreVersionedFunctions()
    {
        await Database.Migrate(202605271010);

        using (Assert.EnterMultipleScope())
        {
            await AssertFunctionExists("logic_for");
        }
    }
}
