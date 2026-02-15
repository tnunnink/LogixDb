using System.Diagnostics;
using LogixDb.Core.Common;
using LogixDb.Testing;

namespace LogixDb.Sqlite.Tests.Snapshots;

[TestFixture]
public class SqliteDbAddSnapshotTests : SqliteTestFixture
{
    [SetUp]
    protected void Setup()
    {
        Database.Migrate();
    }

    //todo L5Sharp should allow new L5X without module catalog
    /*[Test]
    public async Task AddSnapshot_FakeSource_ShouldContainExpectedNumberOFDataTypesRecords()
    {
        var snapshot = Snapshot.Create(TestSource.Fake());

        await Database.AddSnapshot(snapshot);

        //need sql table record count assertions
        //maybe als need table contains record with name assertion
    }*/

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
}