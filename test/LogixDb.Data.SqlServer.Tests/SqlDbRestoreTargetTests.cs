using System.Data;
using LogixDb.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbRestoreTargetTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task Connect_WhenCalled_ShouldReturnOpenConnection()
    {
        using var connection = await Database.Connect();
        Assert.That(connection, Is.Not.Null);
        Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
    }

    [Test]
    public async Task PostTarget_ShouldSaveMetadataButNotRelationalData()
    {
        var target = Target.Create(TestSource.LocalTest());
        
        await Database.PostTarget(target);

        // Verify metadata exists
        var result = await Database.GetTarget(target.TargetKey, target.VersionNumber);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TargetKey, Is.EqualTo(target.TargetKey));

        // Verify relational data does NOT exist
        await AssertRecordDoesNotExist("controller", "instance_id", target.InstanceId);
    }

    [Test]
    public async Task RestoreTarget_ShouldPopulateRelationalDataFromArchivedTarget()
    {
        var target = Target.Create(TestSource.LocalTest());
        await Database.PostTarget(target);
        
        // Retrieve target from database
        var archived = await Database.GetTarget(target.TargetKey, target.VersionNumber);
        Assert.That(archived, Is.Not.Null);

        // Verify relational data does NOT exist yet
        await AssertRecordDoesNotExist("controller", "instance_id", archived.InstanceId);

        await Database.RestoreTarget(archived.TargetKey, archived.VersionNumber);

        // Re-retrieve to get the new InstanceId
        var restored = await Database.GetTarget(archived.TargetKey, archived.VersionNumber);
        Assert.That(restored, Is.Not.Null);
        Assert.That(restored.InstanceId, Is.Not.Zero);

        // Verify relational data now exists
        await AssertRecordExists("controller", "instance_id", restored.InstanceId);
    }

    [Test]
    public async Task RestoreTarget_LatestVersion_ShouldRestoreMostRecent()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.PostTarget(target1);
        
        await Task.Delay(100);

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.PostTarget(target2);

        // Restore latest (version 0)
        await Database.RestoreTarget(target1.TargetKey, 0);

        // Get the latest target to find its InstanceId
        var restored = await Database.GetTarget(target1.TargetKey, 0);
        Assert.That(restored, Is.Not.Null);
        Assert.That(restored.VersionNumber, Is.EqualTo(2));
        Assert.That(restored.InstanceId, Is.Not.Zero);

        // Verify target2 (latest) relational data exists
        await AssertRecordExists("controller", "instance_id", restored.InstanceId);
    }

    [Test]
    public void RestoreTarget_NonExistentTarget_ShouldThrowException()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () => await Database.RestoreTarget("NonExistent", 1));
    }
}
