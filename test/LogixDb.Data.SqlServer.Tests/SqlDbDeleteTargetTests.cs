using LogixDb.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbDeleteTargetTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task DeleteTarget_SingleTarget_ShouldHaveNoTarget()
    {
        var target = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target);

        await Database.DeleteTarget(target.TargetKey);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task DeleteTarget_MultipleTargetDifferentKey_ShouldRemoveOnlyApplicableTargets()
    {
        await Database.ImportTarget(Target.Create(TestSource.LocalTest()));
        await Database.ImportTarget(Target.Create(TestSource.LocalTest(), "CustomTarget"));
        await Database.ImportTarget(Target.Create(TestSource.LocalTest(), "CustomTarget"));

        await Database.DeleteTarget("CustomTarget");

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].TargetKey, Is.EqualTo("controller://TestController"));
    }

    [Test]
    public Task DeleteTarget_NonExistentKey_ShouldNotThrow()
    {
        Assert.DoesNotThrowAsync(async () => await Database.DeleteTarget("NonExistent"));
        return Task.CompletedTask;
    }
}