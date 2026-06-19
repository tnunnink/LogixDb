using System.Diagnostics;
using L5Sharp.Core;
using LogixDb.Testing;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbImportTargetTests : SqliteTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task ImportTarget_LocalTestSource_ShouldReturnValidId()
    {
        var target = Target.Create(TestSource.LocalTest(), "TestProject");

        await Database.ImportTarget(target);

        Assert.That(target.VersionId, Is.GreaterThan(0));
    }

    [Test]
    public async Task ImportTarget_ValidSource_ShouldPopulateTargetTable()
    {
        var target = Target.Create(TestSource.LocalTest(), "TestProject");

        await Database.ImportTarget(target);

        await AssertRecordExists("target", "target_key", target.TargetKey);
    }


    [Test]
    public async Task ImportTarget_SameTargetTwice_ShouldReuseSameTargetId()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Database.ImportTarget(target2);

        await AssertRecordCount("target", 1);
    }

    [Test]
    public async Task ImportTarget_SameTargetTwice_ShouldHaveTwoVersions()
    {
        var target1 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Database.ImportTarget(target1);

        var target2 = Target.Create(TestSource.LocalTest(), "TestProject");
        await Database.ImportTarget(target2);

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task ImportTarget_OverrideTargetKey_ShouldHaveTwoTargetsEntries()
    {
        await Database.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));
        await Database.ImportTarget(Target.Create(TestSource.LocalTest(), "MyCustomKey"));

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(2));

        await AssertRecordExists("target", "target_key", "controller://TestController");
        await AssertRecordExists("target", "target_key", "MyCustomKey");
    }

    [Test]
    public async Task ImportTarget_MultipleSameTarget_ShouldContainSingleControllerRecord()
    {
        await Database.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));
        await Database.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));
        await Database.ImportTarget(Target.Create(TestSource.LocalTest(), "TestProject"));

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(3));

        await AssertRecordCount("controller", 1);
    }

    [Test]
    public async Task ImportTarget_MultipleSameTargetDifferentKey_ShouldContainSingleControllerRecord()
    {
        await Database.ImportTarget(Target.Create(TestSource.LocalTest(), "First"));
        await Database.ImportTarget(Target.Create(TestSource.LocalTest(), "Second"));
        await Database.ImportTarget(Target.Create(TestSource.LocalTest(), "Third"));

        var result = (await Database.ListTargets()).ToArray();
        Assert.That(result, Has.Length.EqualTo(3));

        await AssertRecordCount("controller", 1);
    }

    [Test]
    public async Task ImportTarget_FakeSource_ShouldContainExpectedNumberOFDataTypesRecords()
    {
        var target = Target.Create(TestSource.Custom(c =>
        {
            c.DataTypes.Add(new DataType("TestType") { Description = "This is a test" });
        }), "TestProject");

        await Database.ImportTarget(target);

        await AssertRecordExists("data_type", "type_name", "TestType");
    }

    [Test]
    public async Task ImportTarget_LocalExampleSource_ShouldReturnValidId()
    {
        var target = Target.Create(TestSource.LocalExample(), "TestProject");

        var stopwatch = Stopwatch.StartNew();
        await Database.ImportTarget(target);
        stopwatch.Stop();

        Console.WriteLine(stopwatch.ElapsedMilliseconds);
        Assert.That(target.VersionId, Is.GreaterThan(0));
    }

    [Test]
    public async Task ImportTarget_WithMetadata_ShouldPopulateTargetInfoTable()
    {
        var target = Target.Create(TestSource.LocalTest(), "TestProject");
        target.Info.Add("TestKey", "TestValue");

        await Database.ImportTarget(target);

        await AssertRecordExists("target_info", "property_name", "TestKey");
        await AssertRecordExists("target_info", "property_value", "TestValue");
    }
}