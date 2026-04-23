using Dapper;
using LogixDb.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbTargetSpaceTests : SqlServerTestFixture
{
    [SetUp]
    public async Task Setup()
    {
        // Migrate with None to ensure only the target/version tables exist.
        // This prevents "shredding" data into relational tables during import.
        await Database.Migrate(ComponentOptions.None);
    }

    [Test]
    [Explicit("This test takes some time and is more of a benchmark for growth measurement.")]
    public async Task MeasureTargetSpaceGrowth()
    {
        // 1. Initial State
        var initialSize = await GetDatabaseSizeInBytes();
        Console.WriteLine($"Initial Database Size (Schema Only): {initialSize / 1024.0 / 1024.0:F2} MB");

        // 2. Import Multiple Targets
        const int iterations = 5; // Reduced from 10 for SQL Server speed
        var previousSize = initialSize;

        for (var i = 1; i <= iterations; i++)
        {
            var target = Target.Create(TestSource.LocalExample());
            await Database.ImportTarget(target);

            var currentSize = await GetDatabaseSizeInBytes();
            var delta = currentSize - previousSize;

            Console.WriteLine(
                $"Import {i} completed: Total Size = {currentSize / 1024.0 / 1024.0:F2} MB (+{delta / 1024.0 / 1024.0:F2} MB)");

            previousSize = currentSize;
        }

        // 3. Final Verification
        var finalSize = await GetDatabaseSizeInBytes();
        var totalGrowth = finalSize - initialSize;
        var averageGrowth = totalGrowth / iterations;

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine($"Total Growth after {iterations} imports: {totalGrowth / 1024.0 / 1024.0:F2} MB");
        Console.WriteLine($"Average size per target: {averageGrowth / 1024.0 / 1024.0:F2} MB");

        var targets = await Database.ListTargets();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(targets.Count(), Is.GreaterThanOrEqualTo(iterations));
            Assert.That(finalSize, Is.GreaterThan(initialSize), "Database size should increase after imports.");
        }
    }

    private async Task<long> GetDatabaseSizeInBytes()
    {
        using var connection = await Database.Connect();
        // size is in 8KB pages
        return await connection.QuerySingleAsync<long>("SELECT SUM(size) * 8192 FROM sys.database_files");
    }
}