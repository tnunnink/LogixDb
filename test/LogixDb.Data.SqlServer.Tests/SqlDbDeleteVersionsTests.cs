using LogixDb.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbDeleteVersionsTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Migrator.Migrate(Connection);
    }

    [Test]
    public async Task TruncateTarget_ByVersion_ShouldRemovePreviousVersions()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target2);

        await Manager.DeleteVersions(target1.TargetKey, 2);

        var result = (await Manager.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].VersionNumber, Is.EqualTo(2));
    }

    [Test]
    public async Task TruncateTarget_ByDate_ShouldRemoveOlderVersions()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target1);

        await Task.Delay(1000);
        var cutoff = DateTime.Now;
        await Task.Delay(1000);

        var target2 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target2);

        await Manager.DeleteVersions(target1.TargetKey, cutoff);

        var result = (await Manager.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].VersionId, Is.EqualTo(target2.VersionId));
    }

    [Test]
    public async Task TruncateTarget_NonExistentKey_ShouldNotThrow()
    {
        Assert.DoesNotThrowAsync(async () => await Manager.DeleteVersions("NonExistent", 1));
        Assert.DoesNotThrowAsync(async () => await Manager.DeleteVersions("NonExistent", DateTime.Now));
    }
}
