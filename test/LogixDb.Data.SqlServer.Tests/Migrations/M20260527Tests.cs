namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260527Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202605271009_CreatesCoreVersionedFunctions()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertFunctionExists("logix", "controllers_at_version");
            await AssertFunctionExists("logix", "data_types_at_version");
            await AssertFunctionExists("logix", "aois_at_version");
            await AssertFunctionExists("logix", "modules_at_version");
            await AssertFunctionExists("logix", "tags_at_version");
            await AssertFunctionExists("logix", "tasks_at_version");
            await AssertFunctionExists("logix", "programs_at_version");
            await AssertFunctionExists("logix", "routines_at_version");
            await AssertFunctionExists("logix", "rungs_at_version");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202605271010_CreatesCoreVersionedFunctions()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertFunctionExists("logix", "logic_at_version");
        }
    }
}