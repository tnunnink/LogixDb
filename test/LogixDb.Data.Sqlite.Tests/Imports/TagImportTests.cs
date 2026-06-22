using L5Sharp.Core;
using LogixDb.Testing;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Tests.Imports;

[TestFixture]
public class TagImportTests : SqliteTestFixture
{
    [Test]
    public async Task ImportTarget_WithControllerTag_ShouldContainExpectedRecords()
    {
        var tag = new Tag
        {
            Name = "ControllerTag",
            Value = 123,
            Description = "A controller scoped tag",
            ExternalAccess = Access.ReadOnly
        };
        var source = TestSource.Custom(c => { c.Tags.Add(tag); });
        var target = Target.Create(source, "TestProject");

        await Manager.ImportTarget(target);

        await AssertRecordCount("tag", 1);
        await AssertRecordExists("tag", "tag_name", "ControllerTag");
        await AssertRecordExists("tag", "tag_description", "A controller scoped tag");
        await AssertRecordExists("tag", "data_type", "DINT");
        await AssertRecordExists("tag", "external_access", "ReadOnly");
    }

    [Test]
    public async Task ImportTarget_WithProgramTag_ShouldContainExpectedRecords()
    {
        var source = TestSource.Custom(c =>
        {
            c.Add(Tag.Named("Program:MainProgram.LocalDint").WithValue(123).Build());
        });
        
        var target = Target.Create(source, "TestProject");

        await Manager.ImportTarget(target);

        await AssertRecordCount("tag", 1);
        await AssertRecordExists("tag", "tag_name", "LocalDint");
        await AssertRecordExists("tag", "program_name", "MainProgram");
        await AssertRecordExists("tag", "data_type", "DINT");
    }

    [Test]
    public async Task ImportTarget_TwiceWithSameTag_ShouldHaveExpectedCount()
    {
        var source = TestSource.Custom(c => { c.Tags.Add(new Tag { Name = "TestTag", Value = 100 }); });

        await Manager.ImportTarget(Target.Create(source, "Project1"));
        await Manager.ImportTarget(Target.Create(source, "Project2"));

        await AssertRecordCount("tag", 1);
        await AssertRecordExists("tag", "tag_name", "TestTag");
    }

    [Test]
    public async Task ImportTarget_ModifiedTagDescription_ShouldResultInTwoRecords()
    {
        var source1 = TestSource.Custom(c => c.Tags.Add(new Tag { Name = "TagA", Description = "Old" }));
        await Manager.ImportTarget(Target.Create(source1, "P1"));

        var source2 = TestSource.Custom(c => c.Tags.Add(new Tag { Name = "TagA", Description = "New" }));
        await Manager.ImportTarget(Target.Create(source2, "P1"));

        await AssertRecordCount("tag", 2);
    }
}