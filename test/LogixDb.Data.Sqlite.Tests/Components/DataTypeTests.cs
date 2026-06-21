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
        await AssertRecordExists("data_type", "type_family", "None");
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
    
    [Test]
    public async Task ImportTarget_TwiceWithSameType_ShouldHaveExpectedCount()
    {
        var source = TestSource.Custom(c =>
        {
            c.DataTypes.Add(new DataType("TestType") { Description = "This is a test" });
        });
        
        await Manager.ImportTarget(Target.Create(source, "Project1"));
        await Manager.ImportTarget(Target.Create(source, "Project2"));

        await AssertRecordCount("data_type", 1);
        await AssertRecordExists("data_type", "type_name", "TestType");
        await AssertRecordCount("target_version_map", 4); // 2 types and 2 controllers.
    }
    
    [Test]
    public async Task ImportTarget_ModifiedComponent_ShouldResultInTwoRecordsButOneComponentId()
    {
        var source1 = TestSource.Custom(c => c.DataTypes.Add(new DataType("TypeA") { Description = "Old" }));
        await Manager.ImportTarget(Target.Create(source1, "P1"));

        var source2 = TestSource.Custom(c => c.DataTypes.Add(new DataType("TypeA") { Description = "New" }));
        await Manager.ImportTarget(Target.Create(source2, "P1"));
        
        await AssertRecordCount("data_type", 2);
    }
    
    [Test]
    public async Task ImportTarget_WithDataTypeMembers_ShouldContainMemberRecords()
    {
        var dataType = new DataType("StructureType")
            .AddMember("Field1", "DINT")
            .AddMember("Field2", "BOOL");

        var source = TestSource.Custom(c => c.DataTypes.Add(dataType));
        var target = Target.Create(source, "TestProject");
        await Manager.ImportTarget(target);

        await AssertRecordCount("data_type", 1);
        await AssertRecordExists("data_type", "type_name", "StructureType");
        await AssertRecordCount("data_type_member", 2);
        await AssertRecordExists("data_type_member", "member_name", "Field1");
        await AssertRecordExists("data_type_member", "member_name", "Field2");
    }

    [Test]
    public async Task ImportTarget_StringDataType_ShouldHaveExpectedClass()
    {
        var source = TestSource.Custom(c =>
        {
            c.DataTypes.Add(new DataType("TestType") { Family = DataTypeFamily.String });
        });

        var target = Target.Create(source, "TestProject");
        await Manager.ImportTarget(target);

        await AssertRecordCount("data_type", 1);
        await AssertRecordExists("data_type", "type_family", "String");
    }

    [Test]
    public async Task ImportTarget_MultipleVersionsOfSameType_ShouldTrackVersionHistory()
    {
        var source1 = TestSource.Custom(c => c.DataTypes.Add(new DataType("VersionedType") { Description = "V1" }));
        await Manager.ImportTarget(Target.Create(source1, "Project"));

        var source2 = TestSource.Custom(c => c.DataTypes.Add(new DataType("VersionedType") { Description = "V2" }));
        await Manager.ImportTarget(Target.Create(source2, "Project"));

        var source3 = TestSource.Custom(c => c.DataTypes.Add(new DataType("VersionedType") { Description = "V3" }));
        await Manager.ImportTarget(Target.Create(source3, "Project"));

        await AssertRecordCount("data_type", 3);
    }

    [Test]
    public async Task ImportTarget_WithNestedDataType_ShouldImportSuccessfully()
    {
        var innerType = new DataType("InnerType").AddMember("Value", "DINT");
        var outerType = new DataType("OuterType").AddMember("Inner", "InnerType");

        var source = TestSource.Custom(c =>
        {
            c.DataTypes.Add(innerType);
            c.DataTypes.Add(outerType);
        });

        var target = Target.Create(source, "TestProject");
        await Manager.ImportTarget(target);

        await AssertRecordCount("data_type", 2);
        await AssertRecordExists("data_type", "type_name", "InnerType");
        await AssertRecordExists("data_type", "type_name", "OuterType");
    }
}