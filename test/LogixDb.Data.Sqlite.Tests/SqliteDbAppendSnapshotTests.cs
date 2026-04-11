using LogixDb.Testing;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbAppendSnapshotTests : SqliteTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task AppendSnapshot_LocalTestSource_ShouldReturnValidId()
    {
        var snapshot = Snapshot.Create(TestSource.LocalTest());

        await Database.AppendSnapshot(snapshot);

        Assert.That(snapshot.SnapshotId, Is.GreaterThan(0));
    }

    [Test]
    public async Task AppendSnapshot_MultipleTimes_ShouldKeepAllSnapshots()
    {
        var snapshot1 = Snapshot.Create(TestSource.LocalTest());
        await Database.AppendSnapshot(snapshot1);

        var snapshot2 = Snapshot.Create(TestSource.LocalTest());
        await Database.AppendSnapshot(snapshot2);

        var result = (await Database.ListSnapshots()).ToArray();
        Assert.That(result, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task AppendSnapshot_MultipleTimes_ShouldHaveContentForAllSnapshots()
    {
        var snapshot1 = Snapshot.Create(TestSource.LocalTest());
        await Database.AppendSnapshot(snapshot1);

        var snapshot2 = Snapshot.Create(TestSource.LocalTest());
        await Database.AppendSnapshot(snapshot2);

        await AssertRecordCount("snapshot", 2);
        
        // Check content for both
        await AssertRecordExists("controller", "snapshot_id", snapshot1.SnapshotId);
        await AssertRecordExists("controller", "snapshot_id", snapshot2.SnapshotId);
    }
}
