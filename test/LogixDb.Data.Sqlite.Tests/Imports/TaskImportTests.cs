using L5Sharp.Core;
using LogixDb.Testing;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Tests.Imports;

[TestFixture]
public class TaskImportTests : SqliteTestFixture
{
    [Test]
    public async Task ImportTarget_WithSingleTask_ShouldContainExpectedRecords()
    {
        var task = new L5Sharp.Core.Task("TestTask")
        {
            Description = "This is a test task",
            Type = TaskType.Periodic,
            Priority = 10,
            Rate = 20,
            Watchdog = 500
        };
        var source = TestSource.Custom(c => { c.Tasks.Add(task); });
        var target = Target.Create(source, "TestProject");

        await Manager.ImportTarget(target);

        await AssertRecordCount("task", 1);
        await AssertRecordExists("task", "task_name", "TestTask");
        await AssertRecordExists("task", "task_description", "This is a test task");
        await AssertRecordExists("task", "task_type", "Periodic");
        await AssertRecordExists("task", "priority", 10);
        await AssertRecordExists("task", "scan_rate", 20);
        await AssertRecordExists("task", "watchdog", 500);
    }

    [Test]
    public async Task ImportTarget_WithTwoTasks_ShouldHaveExpectedCountAndRecords()
    {
        var source = TestSource.Custom(c =>
        {
            c.Tasks.Add(new L5Sharp.Core.Task("FirstTask"));
            c.Tasks.Add(new L5Sharp.Core.Task("SecondTask"));
        });

        var target = Target.Create(source, "TestProject");
        await Manager.ImportTarget(target);

        await AssertRecordCount("task", 2);
        await AssertRecordExists("task", "task_name", "FirstTask");
        await AssertRecordExists("task", "task_name", "SecondTask");
    }

    [Test]
    public async Task ImportTarget_TwiceWithSameTask_ShouldHaveExpectedCount()
    {
        var source = TestSource.Custom(c => { c.Tasks.Add(new L5Sharp.Core.Task("TestTask")); });

        await Manager.ImportTarget(Target.Create(source, "Project1"));
        await Manager.ImportTarget(Target.Create(source, "Project2"));

        await AssertRecordCount("task", 1);
        await AssertRecordExists("task", "task_name", "TestTask");
        await AssertRecordCount("target_version_map", 4); // 2 types and 2 controllers.
    }

    [Test]
    public async Task ImportTarget_ModifiedTask_ShouldResultInTwoRecords()
    {
        var source1 = TestSource.Custom(c => c.Tasks.Add(new L5Sharp.Core.Task("TaskA") { Description = "Old" }));
        await Manager.ImportTarget(Target.Create(source1, "P1"));

        var source2 = TestSource.Custom(c => c.Tasks.Add(new L5Sharp.Core.Task("TaskA") { Description = "New" }));
        await Manager.ImportTarget(Target.Create(source2, "P1"));

        await AssertRecordCount("task", 2);
    }
}