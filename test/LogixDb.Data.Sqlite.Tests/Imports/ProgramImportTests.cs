using L5Sharp.Core;
using LogixDb.Testing;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Tests.Imports;

[TestFixture]
public class ProgramImportTests : SqliteTestFixture
{
    [Test]
    public async Task ImportTarget_WithSingleProgram_ShouldContainExpectedRecords()
    {
        var source = TestSource.With<Program>(p =>
        {
            p.Name = "TestProgram";
            p.Description = "This is a test program";
            p.MainRoutineName = "Main";
            p.Disabled = true;
        });

        var target = Target.Create(source, "TestProject");

        await Manager.ImportTarget(target);

        await AssertRecordCount("program", 1);
        await AssertRecordExists("program", "program_name", "TestProgram");
        await AssertRecordExists("program", "program_description", "This is a test program");
        await AssertRecordExists("program", "main_routine", "Main");
        await AssertRecordExists("program", "is_disabled", 1);
    }

    [Test]
    public async Task ImportTarget_WithProgramAttachedToTask_ShouldContainTaskName()
    {
        var source = TestSource.Custom(c =>
        {
            c.Programs.Add(new Program("MyProgram"));
            c.Tasks.Add(new L5Sharp.Core.Task("MainTask") { ScheduledPrograms = [new ScheduledProgram("MyProgram")] });
        });
        var target = Target.Create(source, "TestProject");

        await Manager.ImportTarget(target);

        await AssertRecordCount("program", 1);
        await AssertRecordExists("program", "program_name", "MyProgram");
        await AssertRecordExists("program", "task_name", "MainTask");
    }

    [Test]
    public async Task ImportTarget_TwiceWithSameProgram_ShouldHaveExpectedCount()
    {
        var source = TestSource.Custom(c => { c.Programs.Add(new Program { Name = "TestProgram" }); });

        await Manager.ImportTarget(Target.Create(source, "Project1"));
        await Manager.ImportTarget(Target.Create(source, "Project2"));

        await AssertRecordCount("program", 1);
        await AssertRecordExists("program", "program_name", "TestProgram");
    }

    [Test]
    public async Task ImportTarget_ModifiedProgram_ShouldResultInTwoRecords()
    {
        var source1 = TestSource.Custom(c => c.Programs.Add(new Program { Name = "ProgramA", Description = "Old" }));
        await Manager.ImportTarget(Target.Create(source1, "P1"));

        var source2 = TestSource.Custom(c => c.Programs.Add(new Program { Name = "ProgramA", Description = "New" }));
        await Manager.ImportTarget(Target.Create(source2, "P1"));

        await AssertRecordCount("program", 2);
    }
}