using L5Sharp.Core;
using LogixDb.Testing;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Tests.Components;

[TestFixture]
public class DataTypeTests : SqliteTestFixture
{
    [Test]
    public async Task ImportTarget_WithSingleDataType_ShouldContainExpectedRecords()
    {
        var source = TestSource.Custom(c =>
        {
            c.DataTypes.Add(new DataType("TestType") { Description = "This is a test" });
        });

        var target = Target.Create(source, "TestProject");
        await Manager.ImportTarget(target);

        await AssertRecordCount("data_type", 1);
        await AssertRecordExists("data_type", "type_name", "TestType");
        await AssertRecordExists("data_type", "type_class", "User");
        await AssertRecordExists("data_type", "type_description", "This is a test");
    }

    [Test]
    public async Task ImportTarget_WithTwoTypes_ShouldHaveExpectedCountAndRecords()
    {
        var source = TestSource.Custom(c =>
        {
            c.DataTypes.Add(new DataType("FirstType") { Description = "This is a test" });
            c.DataTypes.Add(new DataType("SecondType") { Description = "This is a test" });
        });

        var target = Target.Create(source, "TestProject");
        await Manager.ImportTarget(target);

        await AssertRecordCount("data_type", 2);
        await AssertRecordExists("data_type", "type_name", "FirstType");
        await AssertRecordExists("data_type", "type_name", "SecondType");
    }
}