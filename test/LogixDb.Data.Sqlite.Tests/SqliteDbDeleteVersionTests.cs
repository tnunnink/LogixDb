using LogixDb.Testing;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbDeleteVersionTests : SqliteTestFixture
{
    [Test]
    public async Task DeleteVersion_SpecificVersion_ShouldRemoveOnlyThatVersion()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target1); // Version 1

        var target2 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target2); // Version 2

        await Manager.DeleteVersion(target1.TargetKey, 1);

        var result = (await Manager.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].VersionNumber, Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteVersion_MultipleTargets_ShouldOnlyRemoveTargetVersion()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "OtherTarget");
        await Manager.ImportTarget(target2);

        await Manager.DeleteVersion(target1.TargetKey, 1);

        var result = (await Manager.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].TargetKey, Is.EqualTo(target2.TargetKey));
    }

    [Test]
    public Task DeleteVersion_NonExistent_ShouldNotThrow()
    {
        Assert.DoesNotThrowAsync(async () => await Manager.DeleteVersion("NonExistent", 1));
        return Task.CompletedTask;
    }
}