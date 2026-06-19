using LogixDb.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbListTargetTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Migrator.Migrate(Connection);
    }
    
    [Test]
    public async Task ListTargets_EmptyDatabase_ShouldReturnEmpty()
    {
        var result = (await Manager.ListTargets()).ToArray();

        Assert.That(result, Is.Empty);
    }
    
    [Test]
    public async Task ListTargets_HasSingleTarget_ShouldHaveExpectedCount()
    {
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));

        var result = (await Manager.ListTargets()).ToArray();

        Assert.That(result, Has.Length.EqualTo(1));
    }
    
    [Test]
    public async Task ListTargets_MultipleTargets_ShouldHaveExpectedCount()
    {
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));

        var result = (await Manager.ListTargets()).ToArray();

        Assert.That(result, Has.Length.EqualTo(3));
    }
    
    [Test]
    public async Task ListTargets_FilterByTargetKey_ShouldReturnMatchingOnly()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "Controller://DifferentTarget");
        await Manager.ImportTarget(target2);

        var result = (await Manager.ListTargets(target1.TargetKey)).ToArray();

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].TargetKey, Is.EqualTo(target1.TargetKey));
    }
    
    [Test]
    public async Task ListTargets_FilterByNonExistentTargetKey_ShouldReturnEmpty()
    {
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));

        var result = (await Manager.ListTargets("nonexistent://target")).ToArray();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ListTargets_MultipleTargetsSameTarget_ShouldReturnAll()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target2);

        var result = (await Manager.ListTargets(target1.TargetKey)).ToArray();

        Assert.That(result, Has.Length.EqualTo(2));
    }
}
