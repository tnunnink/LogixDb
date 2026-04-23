using LogixDb.Testing;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbTargetSpaceTests : SqliteTestFixture
{
    [SetUp]
    public async Task Setup()
    {
        // Migrate with None to ensure only the target/version tables exist.
        // This prevents "shredding" data into relational tables during import.
        await Database.Migrate(ComponentOptions.None);
    }

    [Test]
    public async Task MeasureTargetSpaceGrowth()
    {
        // 1. Initial State
        var initialSize = await GetDatabaseSize();
        Console.WriteLine($"Initial Database Size (Schema Only): {initialSize:F2} MB");

        // 2. Import Multiple Targets
        const int iterations = 10;
        var previousSize = initialSize;

        for (var i = 1; i <= iterations; i++)
        {
            // Use LocalExample() for a more realistic L5X file size
            var target = Target.Create(TestSource.LocalExample());
            
            await Database.ImportTarget(target);

            var currentSize = await GetDatabaseSize();
            var delta = currentSize - previousSize;
            
            Console.WriteLine($"Import {i} completed: Total Size = {currentSize:F2} MB (+{delta:F2} MB)");
            
            previousSize = currentSize;
        }

        // 3. Final Verification
        var finalSize = await GetDatabaseSize();
        var totalGrowth = finalSize - initialSize;
        var averageGrowth = totalGrowth / iterations;

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine($"Total Growth after {iterations} imports: {totalGrowth:F2} MB");
        Console.WriteLine($"Average size per target: {averageGrowth:F2} MB");

        Assert.That(finalSize, Is.GreaterThan(initialSize), "Database size should increase after imports.");
    }
}
