using LogixDb.Testing;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbArchiveTargetTests : SqliteTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task ArchiveTarget_ShouldRemoveRelationalDataButKeepMetadata()
    {
        var target = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target);

        // Verify relational data exists
        await AssertRecordExists("controller", "instance_id", target.InstanceId);

        await Database.ArchiveTarget(target.TargetKey, target.VersionNumber);

        // Verify relational data is gone
        await AssertRecordDoesNotExist("controller", "instance_id", target.InstanceId);

        // Verify target still exists in repository
        var result = await Database.GetTarget(target.TargetKey, target.VersionNumber);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public Task ArchiveTarget_NonExistentTarget_ShouldThrowException()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () => await Database.ArchiveTarget("NonExistent", 1));
        return Task.CompletedTask;
    }
}