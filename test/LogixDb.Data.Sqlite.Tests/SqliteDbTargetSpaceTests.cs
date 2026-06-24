using LogixDb.Testing;
using LogixDb.Testing.Sqlite;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbTargetSpaceTests
{
    private readonly SqliteTestDatabase _database = new();

    [SetUp]
    public Task SetUp()
    {
        return _database.BuildAsync(new SqliteMigrator());
    }

    [TearDown]
    public void Cleanup()
    {
        _database.Dispose();
    }

    [Test]
    public async Task MeasureTargetSpaceGrowth()
    {
        var filePath = _database.Connection.Source;
        var manager = new DbManager(new SqliteProvider(_database.Connection));

        var initialSize = await GetDatabaseSize(filePath);
        Console.WriteLine($"Initial Database Size (Schema Only): {initialSize:F2} MB");

        const int iterations = 5;
        var previousSize = initialSize;

        for (var i = 1; i <= iterations; i++)
        {
            var target = Target.Create(TestSource.LocalExample(), "TestProject");

            await manager.ImportTarget(target);

            var currentSize = await GetDatabaseSize(filePath);
            var delta = currentSize - previousSize;

            Console.WriteLine($"Import {i} completed: Total Size = {currentSize:F2} MB (+{delta:F2} MB)");

            previousSize = currentSize;
        }

        var finalSize = await GetDatabaseSize(filePath);
        var totalGrowth = finalSize - initialSize;
        var averageGrowth = totalGrowth / iterations;

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine($"Total Growth after {iterations} imports: {totalGrowth:F2} MB");
        Console.WriteLine($"Average size per target: {averageGrowth:F2} MB");

        Assert.That(finalSize, Is.GreaterThan(initialSize), "Database size should increase after imports.");
    }

    /// <summary>
    /// Calculates the size of the specified database file in megabytes (MB).
    /// </summary>
    /// <param name="filePath">The file path of the database whose size will be calculated.</param>
    /// <returns>The size of the database file, rounded to two decimal places, in megabytes (MB).</returns>
    private static Task<decimal> GetDatabaseSize(string filePath)
    {
        var size = new FileInfo(filePath).Length;
        return Task.FromResult(Math.Round((decimal)size / 1024 / 1024, 2));
    }
}