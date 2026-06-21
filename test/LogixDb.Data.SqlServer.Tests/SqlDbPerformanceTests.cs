using System.Diagnostics;
using Dapper;
using LogixDb.Testing;
using Microsoft.Data.SqlClient;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbPerformanceTests : SqlServerTestFixture
{
    [SetUp]
    public async Task Setup()
    {
        await Migrator.Migrate(Connection);
    }

    [Test]
    public void CreateFake()
    {
        var fake = TestSource.Fake(1000);
        
        Assert.That(fake, Is.Not.Null);
    } 

    [Test]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(10000)]
    [TestCase(100000)]
    [Explicit("Still WIP - eventually want to stress test queries on high load to ensure index performance.")]
    public async Task ExecuteQuery_AfterLargeNumberOfFakeImports_ShouldBePerformant(int count)
    {
        var target = Target.Create(TestSource.Fake(count), "TestProject");
        await Manager.ImportTarget(target);

        await using var connection = new SqlConnection(Connection.ToConnectionString());
        await connection.OpenAsync();
        
        // We want to test the GetVersionedTags function
        var version = await connection.QuerySingleAsync<int>("SELECT MAX(version_id) FROM logix.target_version");

        var sw = Stopwatch.StartNew();
        var tags = await connection.QueryAsync("SELECT * FROM logix.tags_at_version(@version)", new { version });
        sw.Stop();

        var recordCount = tags.Count();
        Console.WriteLine($"[DEBUG_LOG] Query for Version {version} returned {recordCount} tags.");
        Console.WriteLine($"[DEBUG_LOG] Time taken: {sw.ElapsedMilliseconds} ms");
        
        Assert.That(recordCount, Is.GreaterThan(0));
    }
}