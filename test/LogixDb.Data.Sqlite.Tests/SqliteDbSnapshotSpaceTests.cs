using LogixDb.Testing;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbSnapshotSpaceTests : SqliteTestFixture
{
    [SetUp]
    public async Task Setup()
    {
        // Migrate with None to ensure only the snapshot/target tables exist.
        // This prevents "shredding" data into relational tables during import.
        await Database.Migrate(ComponentOptions.None);
    }

    [Test]
    public async Task MeasureSnapshotSpaceGrowth()
    {
        // 1. Initial State
        var initialSize = new FileInfo(TempDb).Length;
        Console.WriteLine($"Initial Database Size (Schema Only): {initialSize / 1024.0:F2} KB");

        // 2. Import Multiple Snapshots
        const int iterations = 10;
        var previousSize = initialSize;

        for (var i = 1; i <= iterations; i++)
        {
            // Use LocalExample() for a more realistic L5X file size
            var snapshot = Snapshot.Create(TestSource.LocalExample());
            
            await Database.ArchiveSnapshot(snapshot);

            var currentSize = new FileInfo(TempDb).Length;
            var delta = currentSize - previousSize;
            
            Console.WriteLine($"Import {i}: Total Size = {currentSize / 1024.0:F2} KB (+{delta / 1024.0:F2} KB)");
            
            previousSize = currentSize;
        }

        // 3. Final Verification
        var finalSize = new FileInfo(TempDb).Length;
        var totalGrowth = finalSize - initialSize;
        var averageGrowth = totalGrowth / iterations;

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine($"Total Growth after {iterations} imports: {totalGrowth / 1024.0:F2} KB");
        Console.WriteLine($"Average size per snapshot: {averageGrowth / 1024.0:F2} KB");

        Assert.That(finalSize, Is.GreaterThan(initialSize), "Database size should increase after imports.");
    }
}
