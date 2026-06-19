using LogixDb.Data;
using LogixDb.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbDeleteVersionTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task DeleteVersion_SpecificVersion_ShouldRemoveOnlyThatVersion()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Database.ImportTarget(target1); // Version 1

        var target2 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Database.ImportTarget(target2); // Version 2

        await Database.DeleteVersion(target1.TargetKey, 1);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].VersionNumber, Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteVersion_MultipleTargets_ShouldOnlyRemoveTargetVersion()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "OtherTarget");
        await Database.ImportTarget(target2);

        await Database.DeleteVersion(target1.TargetKey, 1);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].TargetKey, Is.EqualTo(target2.TargetKey));
    }

    [Test]
    public Task DeleteVersion_NonExistent_ShouldNotThrow()
    {
        Assert.DoesNotThrowAsync(async () => await Database.DeleteVersion("NonExistent", 1));
        return Task.CompletedTask;
    }
}
