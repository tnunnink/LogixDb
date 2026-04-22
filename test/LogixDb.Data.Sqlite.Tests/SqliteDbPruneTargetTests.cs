using LogixDb.Testing;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbPruneTargetTests : SqliteTestFixture
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
        
        // Manual restore to have two instances (ImportTarget prunes previous instances of same target by default in its current implementation)
        // Actually, current ImportTarget implementation:
        // await PostTargetVersionAsync(target, token);
        // await ExecuteSqlScriptAsync(SqliteScript.DeleteTargetInstances, new { target.TargetKey }, token);
        // await RestoreTargetVersionAsync(target, token);
        // So it already prunes.
        
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
