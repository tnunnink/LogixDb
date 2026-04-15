using LogixDb.Testing;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbGetSnapshotTests : SqliteTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task GetSnapshot_LatestVersionButContainsSingleSnapshot_ShouldNotBeNull()
    {
        var snapshot = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot);

        var result = await Database.GetSnapshot(snapshot.TargetKey);

        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.SnapshotId, Is.EqualTo(snapshot.SnapshotId));
            Assert.That(result.TargetKey, Is.EqualTo(snapshot.TargetKey));
        }
    }
    
    [Test]
    public async Task GetSnapshot_LatestVersionAndContainsMultipleSnapshot_ShouldReturnLatest()
    {
        var snapshot1 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot1);

        await Task.Delay(1000); // Ensure different timestamps

        var snapshot2 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot2);

        var result = await Database.GetSnapshot(snapshot1.TargetKey);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.SnapshotId, Is.EqualTo(snapshot2.SnapshotId));
    }

    [Test]
    public async Task GetSnapshot_ByVersionExistingSnapshot_ShouldReturnSnapshot()
    {
        var snapshot = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot);

        var result = await Database.GetSnapshot(snapshot.TargetKey, 1);

        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.SnapshotId, Is.EqualTo(snapshot.SnapshotId));
            Assert.That(result.TargetKey, Is.EqualTo(snapshot.TargetKey));
        }
    }

    [Test]
    public async Task GetSnapshot_ByVersionMultipleSnapshots_ShouldReturnCorrectOne()
    {
        var snapshot1 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot1);

        var snapshot2 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot2);

        var result = await Database.GetSnapshot(snapshot1.TargetKey, 2);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SnapshotId, Is.GreaterThan(0));
            Assert.That(result.TargetKey, Is.EqualTo(snapshot1.TargetKey));
            Assert.That(result.VersionNumber, Is.EqualTo(2));
        }
    }
}
