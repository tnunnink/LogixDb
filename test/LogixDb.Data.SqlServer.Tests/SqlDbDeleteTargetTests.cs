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
    public async Task DeleteTarget_ByTargetKey_ShouldRemoveAllVersions()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        var target3 = Target.Create(TestSource.LocalTest(), "Controller://CustomTarget");
        await Database.ImportTarget(target3);

        await Database.DeleteTarget(target1.TargetKey);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].TargetKey, Is.EqualTo(target3.TargetKey));
    }

    [Test]
    public Task DeleteTarget_NonExistentKey_ShouldNotThrow()
    {
        Assert.DoesNotThrowAsync(async () => await Database.DeleteTarget("NonExistent"));
        return Task.CompletedTask;
    }
}
