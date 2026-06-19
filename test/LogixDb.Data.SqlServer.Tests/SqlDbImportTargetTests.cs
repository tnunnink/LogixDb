using System.Diagnostics;
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
        await Migrator.Migrate(Connection);
    }

    [Test]
    public async Task ImportTarget_LocalTestSource_ShouldReturnValidId()
    {
        var target = Target.Create(TestSource.LocalTest(), "TestProject");

        await Manager.ImportTarget(target);

        Assert.That(target.VersionId, Is.GreaterThan(0));
    }

    [Test]
    public async Task ImportTarget_ValidSource_ShouldPopulateTargetTable()
    {
        var target = Target.Create(TestSource.LocalTest(), "TestProject");

        await Manager.ImportTarget(target);

        await AssertRecordExists("logix.target", "target_key", target.TargetKey);
    }

    [Test]
    public async Task ImportTarget_SameTargetTwice_ShouldReuseSameTargetId()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target2);

        await AssertRecordCount("logix.target", 1);
    }

    [Test]
    public async Task ImportTarget_SameTargetTwice_ShouldHaveTwoVersions()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target2);

        var result = (await Manager.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task ImportTarget_OverrideTargetKey_ShouldHaveTwoTargetsEntries()
    {
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "MyCustomKey"));

        var result = (await Manager.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(2));

        await AssertRecordExists("logix.target", "target_key", "TestProject");
        await AssertRecordExists("logix.target", "target_key", "MyCustomKey");
    }

    [Test]
    public async Task ImportTarget_MultipleSameTarget_ShouldContainSingleControllerRecord()
    {
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));

        var result = (await Manager.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(3));

        await AssertRecordCount("logix.controller", 1);
    }

    [Test]
    public async Task ImportTarget_MultipleSameTargetDifferentKey_ShouldContainSingleControllerRecord()
    {
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "First"));
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "Second"));
        await Manager.ImportTarget(Target.Create(TestSource.LocalTest(), "Third"));

        var result = (await Manager.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(3));

        await AssertRecordCount("logix.controller", 1);
    }

    [Test]
    public async Task ImportTarget_FakeSource_ShouldContainExpectedNumberOFDataTypesRecords()
    {
        var target = Target.Create(TestSource.Custom(c =>
        {
            c.DataTypes.Add(new DataType("TestType") { Description = "This is a test" });
        }), "TestProject");

        await Manager.ImportTarget(target);

        await AssertRecordExists("logix.data_type", "type_name", "TestType");
    }

    [Test]
    public async Task ImportTarget_LocalExampleSource_ShouldReturnValidId()
    {
        var target = Target.Create(TestSource.LocalExample(), "TestProject");

        var stopwatch = Stopwatch.StartNew();
        await Manager.ImportTarget(target);
        stopwatch.Stop();

        Console.WriteLine(stopwatch.ElapsedMilliseconds);
        Assert.That(target.VersionId, Is.GreaterThan(0));
    }

    [Test]
    public async Task ImportTarget_WithMetadata_ShouldPopulateTargetInfoTable()
    {
        var target = Target.Create(TestSource.LocalTest(), "TestProject");
        target.Info.Add("TestKey", "TestValue");

        await Manager.ImportTarget(target);

        await AssertRecordExists("logix.target_info", "property_name", "TestKey");
        await AssertRecordExists("logix.target_info", "property_value", "TestValue");
    }
}
