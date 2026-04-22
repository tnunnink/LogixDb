using LogixDb.Testing;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbGetTargetTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task GetTarget_LatestVersionButContainsSingleTarget_ShouldNotBeNull()
    {
        var target = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target);

        var result = await Database.GetTarget(target.TargetKey);

        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.InstanceId, Is.EqualTo(target.InstanceId));
            Assert.That(result.TargetKey, Is.EqualTo(target.TargetKey));
        }
    }
    
    [Test]
    public async Task GetTarget_LatestVersionAndContainsMultipleTarget_ShouldReturnLatest()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        await Task.Delay(1000); // Ensure different timestamps

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        var result = await Database.GetTarget(target1.TargetKey);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.InstanceId, Is.EqualTo(target2.InstanceId));
    }

    [Test]
    public async Task GetTarget_ByVersionExistingTarget_ShouldReturnTarget()
    {
        var target = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target);

        var result = await Database.GetTarget(target.TargetKey, 1);

        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.InstanceId, Is.EqualTo(target.InstanceId));
            Assert.That(result.TargetKey, Is.EqualTo(target.TargetKey));
        }
    }

    [Test]
    public async Task GetTarget_ByVersionMultipleTargets_ShouldReturnCorrectOne()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        var result = await Database.GetTarget(target1.TargetKey, 2);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.InstanceId, Is.GreaterThan(0));
            Assert.That(result.TargetKey, Is.EqualTo(target1.TargetKey));
            Assert.That(result.VersionNumber, Is.EqualTo(2));
        }
    }
}
