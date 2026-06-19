using LogixDb.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbDeleteTargetTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Migrator.Migrate(Connection);
    }

    [Test]
    public async Task DeleteTarget_SingleTarget_ShouldHaveNoTarget()
    {
        var target = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target);

        await Manager.DeleteTarget(target.TargetKey);

        var result = (await Manager.ListTargets()).ToArray();
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task DeleteTarget_MultipleTargetDifferentKey_ShouldRemoveOnlyApplicableTargets()
    {
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "CustomTarget"));
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "CustomTarget"));

        await Manager.DeleteTarget("CustomTarget");

        var result = (await Manager.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].TargetKey, Is.EqualTo("TestProject"));
    }

    [Test]
    public Task DeleteTarget_NonExistentKey_ShouldNotThrow()
    {
        Assert.DoesNotThrowAsync(async () => await Manager.DeleteTarget("NonExistent"));
        return Task.CompletedTask;
    }
}