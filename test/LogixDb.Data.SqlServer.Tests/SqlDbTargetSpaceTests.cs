using Dapper;
using LogixDb.Data.Abstractions;
using LogixDb.Testing;
using LogixDb.Testing.SqlServer;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbTargetSpaceTests
{
    private readonly SqlServerTestDatabase _database = new();

    [SetUp]
    public Task SetUp() => _database.BuildAsync(new SqlServerMigrator());

    [TearDown]
    public ValueTask Cleanup() => _database.DisposeAsync();

    [Test]
    public async Task MeasureTargetSpaceGrowth()
    {
        var provider = new SqlServerProvider(_database.Connection);
        var manager = new DbManager(provider);

        var initialSize = await GetDatabaseSize(provider);
        Console.WriteLine($"Initial Database Size (Schema Only): {initialSize} MB");

        const int iterations = 5;
        var previousSize = initialSize;

        for (var i = 1; i <= iterations; i++)
        {
            var target = Target.Create(TestSource.LocalExample(), "TestProject");
            await manager.ImportTarget(target);

            var currentSize = await GetDatabaseSize(provider);
            var delta = currentSize - previousSize;

            Console.WriteLine($"Import {i} completed: Total Size = {currentSize:F2} MB (+{delta:F2} MB)");

            previousSize = currentSize;
        }
        
        var finalSize = await GetDatabaseSize(provider);
        var totalGrowth = finalSize - initialSize;
        var averageGrowth = totalGrowth / iterations;

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine($"Total Growth after {iterations} imports: {totalGrowth:F2} MB");
        Console.WriteLine($"Average size per target: {averageGrowth:F2} MB");

        var targets = await manager.ListTargets();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(targets.Count(), Is.GreaterThanOrEqualTo(iterations));
            Assert.That(finalSize, Is.GreaterThan(initialSize), "Database size should increase after imports.");
        }
    }

    /// <summary>
    /// Retrieves the current size of the database in megabytes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The result contains the size of the database in megabytes as an integer.</returns>
    private static async Task<decimal> GetDatabaseSize(IDbProvider provider)
    {
        await using var connection = await provider.OpenConnection();

        return await connection.QuerySingleAsync<decimal>(
            """
            SELECT CAST(SUM(FILEPROPERTY(name, 'SpaceUsed')) * 8 / 1024.0 AS DECIMAL(10,2)) AS SpaceUsedMB
            FROM sys.database_files
            WHERE type_desc = 'ROWS';
            """);
    }
}