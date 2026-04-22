using LogixDb.Testing;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbTruncateTargetTests : SqliteTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task TruncateTarget_ByVersion_ShouldRemovePreviousVersions()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        await Database.TruncateTarget(target1.TargetKey, 2);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].VersionNumber, Is.EqualTo(2));
    }

    [Test]
    public async Task TruncateTarget_ByDate_ShouldRemoveOlderVersions()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        await Task.Delay(1000);
        var cutoff = DateTime.Now;
        await Task.Delay(1000);

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        await Database.TruncateTarget(target1.TargetKey, cutoff);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].VersionId, Is.EqualTo(target2.VersionId));
    }

    [Test]
    public async Task TruncateTarget_NonExistentKey_ShouldNotThrow()
    {
        Assert.DoesNotThrowAsync(async () => await Database.TruncateTarget("NonExistent", 1));
        Assert.DoesNotThrowAsync(async () => await Database.TruncateTarget("NonExistent", DateTime.Now));
    }
}
