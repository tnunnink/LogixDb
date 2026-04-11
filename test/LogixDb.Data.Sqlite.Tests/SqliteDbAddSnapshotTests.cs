using System.Diagnostics;
using Dapper;
using L5Sharp.Core;
using LogixDb.Testing;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbAddSnapshotTests : SqliteTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task AddSnapshot_LocalTestSource_ShouldReturnValidId()
    {
        var snapshot = Snapshot.Create(TestSource.LocalTest());

        await Database.AddSnapshot(snapshot);

        Assert.That(snapshot.SnapshotId, Is.GreaterThan(0));
    }

    [Test]
    public async Task AddSnapshot_LocalExampleSource_ShouldReturnValidId()
    {
        var snapshot = Snapshot.Create(TestSource.LocalExample());

        var stopwatch = Stopwatch.StartNew();
        await Database.AddSnapshot(snapshot);
        stopwatch.Stop();

        Console.WriteLine(stopwatch.ElapsedMilliseconds);
        Assert.That(snapshot.SnapshotId, Is.GreaterThan(0));
    }


    [Test]
    public async Task AddSnapshot_ExistingSnapshot_ShouldPrunePreviousContent()
    {
        var snapshot1 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot1);

        await Task.Delay(1000); // Ensure different timestamps

        var snapshot2 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot2);

        var result = (await Database.ListSnapshots()).ToArray();
        Assert.That(result, Has.Length.EqualTo(2));

        var snapshots = result.OrderBy(s => s.SnapshotId).ToArray();
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(snapshots[0].SnapshotId, Is.EqualTo(snapshot1.SnapshotId));
            Assert.That(snapshots[1].SnapshotId, Is.EqualTo(snapshot2.SnapshotId));
            
            // Previous should have NO content (pruned)
            await AssertRecordDoesNotExist("controller", "snapshot_id", snapshot1.SnapshotId);
            // Latest should HAVE content
            await AssertRecordExists("controller", "snapshot_id", snapshot2.SnapshotId);
        }
    }

    [Test]
    public async Task AddSnapshot_ExistingSnapshot_ShouldPrunePreviousContentTwice()
    {
        var snapshot1 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot1);

        var snapshot2 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot2);

        var result = (await Database.ListSnapshots()).ToArray();
        Assert.That(result, Has.Length.EqualTo(2));
        
        // Previous should have NO content (pruned)
        await AssertRecordDoesNotExist("controller", "snapshot_id", snapshot1.SnapshotId);
        // Latest should HAVE content
        await AssertRecordExists("controller", "snapshot_id", snapshot2.SnapshotId);
    }

    [Test]
    public async Task AddSnapshot_MultipleExistingSnapshots_ShouldPruneAllPreviousSnapshotsContent()
    {
        var snapshot1 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot1);

        var snapshot2 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot2);

        var snapshot3 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot3);

        var result = (await Database.ListSnapshots()).ToArray();
        Assert.That(result, Has.Length.EqualTo(3));
        
        await AssertRecordDoesNotExist("controller", "snapshot_id", snapshot1.SnapshotId);
        await AssertRecordDoesNotExist("controller", "snapshot_id", snapshot2.SnapshotId);
        await AssertRecordExists("controller", "snapshot_id", snapshot3.SnapshotId);
    }

    [Test]
    public async Task AddSnapshot_DifferentTargets_ShouldOnlyAffectSameTarget()
    {
        var snapshot1 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot1);

        var snapshot2 = Snapshot.Create(TestSource.LocalTest(), "Controller://CustomTarget");
        await Database.AddSnapshot(snapshot2);

        var snapshot3 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot3);

        var result = (await Database.ListSnapshots()).ToArray();
        Assert.That(result, Has.Length.EqualTo(3));
        
        // Target 1: snapshot1 (pruned), snapshot3 (active)
        await AssertRecordDoesNotExist("controller", "snapshot_id", snapshot1.SnapshotId);
        await AssertRecordExists("controller", "snapshot_id", snapshot3.SnapshotId);
        
        // Target 2: snapshot2 (active)
        await AssertRecordExists("controller", "snapshot_id", snapshot2.SnapshotId);
    }

    [Test]
    public async Task AddSnapshot_MultipleSnapshotsDifferentTargets_ShouldOnlyAffectSameTarget()
    {
        var snapshot1 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot1);

        var snapshot2 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot2);

        var snapshot3 = Snapshot.Create(TestSource.LocalTest(), "Controller://CustomTarget");
        await Database.AddSnapshot(snapshot3);

        var snapshot4 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot4);

        var result = (await Database.ListSnapshots()).ToArray();
        Assert.That(result, Has.Length.EqualTo(4));

        // Target 1: snapshot 1, 2, 4. 1 and 2 should be pruned.
        await AssertRecordDoesNotExist("controller", "snapshot_id", snapshot1.SnapshotId);
        await AssertRecordDoesNotExist("controller", "snapshot_id", snapshot2.SnapshotId);
        await AssertRecordExists("controller", "snapshot_id", snapshot4.SnapshotId);

        // Target 2: snapshot 3. Active.
        await AssertRecordExists("controller", "snapshot_id", snapshot3.SnapshotId);
    }

    [Test]
    public async Task AddSnapshot_MultipleTimes_ShouldSetImportDate()
    {
        var snapshot = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot);

        using var connection = await Database.Connect();
        var importDate = await connection.QuerySingleAsync<DateTime>(
            "SELECT import_date FROM snapshot WHERE snapshot_id = @id",
            new { id = snapshot.SnapshotId }
        );

        Assert.That(importDate, Is.GreaterThan(DateTime.MinValue));
        Assert.That(importDate, Is.LessThanOrEqualTo(DateTime.UtcNow));
    }

    [Test]
    public async Task AddSnapshot_ShouldPopulateTargetTable()
    {
        var snapshot = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot);

        await AssertRecordExists("target", "target_key", snapshot.TargetKey);
    }

    [Test]
    public async Task AddSnapshot_WithSameTargetTwice_ShouldReuseSameTargetId()
    {
        var snapshot1 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot1);

        var snapshot2 = Snapshot.Create(TestSource.LocalTest());
        await Database.AddSnapshot(snapshot2);

        await AssertRecordCount("target", 1);
    }

    [Test]
    public async Task AddSnapshot_FakeSource_ShouldContainExpectedNumberOFDataTypesRecords()
    {
        var snapshot = Snapshot.Create(TestSource.Custom(c =>
        {
            c.DataTypes.Add(new DataType("TestType") { Description = "This is a test" });
        }));

        await Database.AddSnapshot(snapshot);

        await AssertRecordExists("data_type", "type_name", "TestType");
    }
    
    
}
