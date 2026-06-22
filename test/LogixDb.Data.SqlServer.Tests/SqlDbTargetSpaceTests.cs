using LogixDb.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbTargetSpaceTests : SqlServerTestFixture
{
    [Test]
    public async Task MeasureTargetSpaceGrowth()
    {
        // 1. Initial State
        var initialSize = await GetDatabaseSize();
        Console.WriteLine($"Initial Database Size (Schema Only): {initialSize} MB");

        // 2. Import Multiple Targets
        const int iterations = 5;
        var previousSize = initialSize;

        for (var i = 1; i <= iterations; i++)
        {
            var target = Target.Create(TestSource.LocalExample(), "TestProject");
            await Manager.ImportTarget(target);

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

        var targets = await Manager.ListTargets();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(targets.Count(), Is.GreaterThanOrEqualTo(iterations));
            Assert.That(finalSize, Is.GreaterThan(initialSize), "Database size should increase after imports.");
        }
    }
}