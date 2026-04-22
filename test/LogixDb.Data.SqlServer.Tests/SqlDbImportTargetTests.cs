using System.Diagnostics;
using Dapper;
using L5Sharp.Core;
using LogixDb.Testing;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbImportTargetTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task ImportTarget_LocalTestSource_ShouldReturnValidId()
    {
        var target = Target.Create(TestSource.LocalTest());

        await Database.ImportTarget(target);

        Assert.That(target.InstanceId, Is.GreaterThan(0));
    }

    [Test]
    public async Task ImportTarget_LocalExampleSource_ShouldReturnValidId()
    {
        var target = Target.Create(TestSource.LocalExample());

        var stopwatch = Stopwatch.StartNew();
        await Database.ImportTarget(target);
        stopwatch.Stop();

        Console.WriteLine(stopwatch.ElapsedMilliseconds);
        Assert.That(target.InstanceId, Is.GreaterThan(0));
    }


    [Test]
    public async Task ImportTarget_ExistingInstances_ShouldPrunePreviousContent()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        await Task.Delay(1000); // Ensure different timestamps

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(2));

        var targets = result.OrderBy(s => s.InstanceId).ToArray();

        using (Assert.EnterMultipleScope())
        {
            // Previous target instance id should now be zero since it was deleted
            Assert.That(targets[0].InstanceId, Is.Zero);
            Assert.That(targets[1].InstanceId, Is.EqualTo(target2.InstanceId));

            // Previous should have NO content (pruned)
            await AssertRecordDoesNotExist("controller", "instance_id", target1.InstanceId);
            // Latest should HAVE content
            await AssertRecordExists("controller", "instance_id", target2.InstanceId);
        }
    }

    [Test]
    public async Task ImportTarget_ExistingTarget_ShouldPrunePreviousContentTwice()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(2));

        // Previous should have NO content (pruned)
        await AssertRecordDoesNotExist("controller", "instance_id", target1.InstanceId);
        // Latest should HAVE content
        await AssertRecordExists("controller", "instance_id", target2.InstanceId);
    }

    [Test]
    public async Task ImportTarget_MultipleExistingTargets_ShouldPruneAllPreviousTargetsContent()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        var target3 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target3);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(3));

        await AssertRecordDoesNotExist("controller", "instance_id", target1.InstanceId);
        await AssertRecordDoesNotExist("controller", "instance_id", target2.InstanceId);
        await AssertRecordExists("controller", "instance_id", target3.InstanceId);
    }

    [Test]
    public async Task ImportTarget_DifferentTargets_ShouldOnlyAffectSameTarget()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "Controller://CustomTarget");
        await Database.ImportTarget(target2);

        var target3 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target3);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(3));

        // Target 1: target1 (pruned), target3 (active)
        await AssertRecordDoesNotExist("controller", "instance_id", target1.InstanceId);
        await AssertRecordExists("controller", "instance_id", target3.InstanceId);

        // Target 2: target2 (active)
        await AssertRecordExists("controller", "instance_id", target2.InstanceId);
    }

    [Test]
    public async Task ImportTarget_MultipleTargetsDifferentTargets_ShouldOnlyAffectSameTarget()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        var target3 = Target.Create(TestSource.LocalTest(), "Controller://CustomTarget");
        await Database.ImportTarget(target3);

        var target4 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target4);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(4));

        // Target 1: target 1, 2, 4. 1 and 2 should be pruned.
        await AssertRecordDoesNotExist("controller", "instance_id", target1.InstanceId);
        await AssertRecordDoesNotExist("controller", "instance_id", target2.InstanceId);
        await AssertRecordExists("controller", "instance_id", target4.InstanceId);

        // Target 2: target 3. Active.
        await AssertRecordExists("controller", "instance_id", target3.InstanceId);
    }

    [Test]
    public async Task ImportTarget_LocalTestTarget_ShouldSetImportDate()
    {
        var target = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target);

        using var connection = (Microsoft.Data.SqlClient.SqlConnection)await Database.Connect();
        var importDate = await connection.QuerySingleAsync<DateTime>(
            "SELECT import_date FROM target_version WHERE version_id = @id",
            new { id = target.VersionId }
        );

        Assert.That(importDate, Is.GreaterThan(DateTime.MinValue));
        Assert.That(importDate, Is.LessThanOrEqualTo(DateTime.UtcNow));
    }

    [Test]
    public async Task ImportTarget_ValidSource_ShouldPopulateTargetTable()
    {
        var target = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target);

        await AssertRecordExists("target", "target_key", target.TargetKey);
    }

    [Test]
    public async Task ImportTarget_SameTargetTwice_ShouldReuseSameTargetId()
    {
        var target1 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target2);

        await AssertRecordCount("target", 1);
    }

    [Test]
    public async Task ImportTarget_FakeSource_ShouldContainExpectedNumberOFDataTypesRecords()
    {
        var target = Target.Create(TestSource.Custom(c =>
        {
            c.DataTypes.Add(new DataType("TestType") { Description = "This is a test" });
        }));

        await Database.ImportTarget(target);

        await AssertRecordExists("data_type", "type_name", "TestType");
    }
}
