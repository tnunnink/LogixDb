using LogixDb.Testing;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbListTargetTests : SqliteTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }
    
    [Test]
    public async Task ListTargets_EmptyDatabase_ShouldReturnEmpty()
    {
        var result = (await Database.ListTargets()).ToArray();

        Assert.That(result, Is.Empty);
    }
    
    [Test]
    public async Task ListTargets_HasSingleTarget_ShouldHaveExpectedCount()
    {
        await Database.ImportTarget(Target.Create(TestSource.LocalTest()));

        var result = (await Database.ListTargets()).ToArray();

        Assert.That(result, Has.Length.EqualTo(1));
    }
    
    [Test]
    public async Task ListTargets_MultipleTargets_ShouldHaveExpectedCount()
    {
        await Database.ImportTarget(Target.Create(TestSource.LocalTest()));
        await Database.ImportTarget(Target.Create(TestSource.LocalTest()));
        await Database.ImportTarget(Target.Create(TestSource.LocalTest()));

        var result = (await Database.ListTargets()).ToArray();

        Assert.That(result, Has.Length.EqualTo(3));
    }
    
    [Test]
    public async Task ListTargets_FilterByTargetKey_ShouldReturnMatchingOnly()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "Controller://DifferentTarget");
        await Database.ImportTarget(target2);

        var result = (await Database.ListTargets(target1.TargetKey)).ToArray();

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].TargetKey, Is.EqualTo(target1.TargetKey));
    }
    
    [Test]
    public async Task ListTargets_FilterByNonExistentTargetKey_ShouldReturnEmpty()
    {
        await Database.ImportTarget(Target.Create(TestSource.LocalTest()));

        var result = (await Database.ListTargets("nonexistent://target")).ToArray();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ListTargets_MultipleTargetsSameTarget_ShouldReturnAll()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        var result = (await Database.ListTargets(target1.TargetKey)).ToArray();

        Assert.That(result, Has.Length.EqualTo(2));
    }
}
