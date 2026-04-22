using LogixDb.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbPruneTargetTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task PruneTarget_ShouldRemoveAllRelationalInstances()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);
        
        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);
        
        await Database.PruneTarget(target1.TargetKey);

        await AssertRecordDoesNotExist("controller", "instance_id", target1.InstanceId);
        await AssertRecordDoesNotExist("controller", "instance_id", target2.InstanceId);
    }

    [Test]
    public async Task PruneTarget_NonExistentTarget_ShouldNotThrow()
    {
        Assert.DoesNotThrowAsync(async () => await Database.PruneTarget("NonExistent"));
    }
}
