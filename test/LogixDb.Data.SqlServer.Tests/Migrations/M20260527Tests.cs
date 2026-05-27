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
            await AssertFunctionExists("GetVersionedControllers");
            await AssertFunctionExists("GetVersionedDataTypes");
            await AssertFunctionExists("GetVersionedAois");
            await AssertFunctionExists("GetVersionedModules");
            await AssertFunctionExists("GetVersionedTags");
            await AssertFunctionExists("GetVersionedTasks");
            await AssertFunctionExists("GetVersionedPrograms");
            await AssertFunctionExists("GetVersionedRoutines");
            await AssertFunctionExists("GetVersionedRungs");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202605271014_CreatesChildVersionedFunctions()
    {
        await Database.Migrate(202605271014);

        using (Assert.EnterMultipleScope())
        {
            await AssertFunctionExists("GetVersionedInstructions");
            await AssertFunctionExists("GetVersionedArguments");
            await AssertFunctionExists("GetVersionedReferences");
            await AssertFunctionExists("GetVersionedTagMembers");
            await AssertFunctionExists("GetVersionedTagComments");
            await AssertFunctionExists("GetVersionedAoiParameters");
            await AssertFunctionExists("GetVersionedDataTypeMembers");
            await AssertFunctionExists("GetVersionedOperands");
        }
    }
}