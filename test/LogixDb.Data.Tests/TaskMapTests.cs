using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;
using Task = L5Sharp.Core.Task;

namespace LogixDb.Data.Tests;

[TestFixture]
public class TaskMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new TaskMap();
        var records = new List<TaskRecord>
        {
            new(1, new Task { Name = "Task1" }),
            new(1, new Task { Name = "Task2" })
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new TaskMap();
        var task = new Task { Name = "TestTask", Description = "Test Description" };
        var record = new TaskRecord(1, task);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["task_name"].Should().Be("TestTask");
        table.Rows[0]["description"].Should().Be("Test Description");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new TaskMap();
        var task = new Task { Name = "TestTask" };
        var record = new TaskRecord(1, task);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}
