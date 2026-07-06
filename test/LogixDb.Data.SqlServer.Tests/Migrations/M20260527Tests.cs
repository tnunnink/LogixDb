namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260527Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202605271009_CreatesCoreVersionedFunctions()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertFunctionExists("logix", "get_controller");
            await AssertFunctionExists("logix", "get_data_types");
            await AssertFunctionExists("logix", "get_aois");
            await AssertFunctionExists("logix", "get_modules");
            await AssertFunctionExists("logix", "get_tags");
            await AssertFunctionExists("logix", "get_tasks");
            await AssertFunctionExists("logix", "get_programs");
            await AssertFunctionExists("logix", "get_routines");
            await AssertFunctionExists("logix", "get_rungs");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202605271010_CreatesCoreVersionedFunctions()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertFunctionExists("logix", "get_logic");
        }
    }
}