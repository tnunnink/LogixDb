using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;
using Task = L5Sharp.Core.Task;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class TaskMapTests
{
    [Test]
    public void GenerateTable_BasicTask_ShouldMapAllProperties()
    {
        var map = new TaskMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("task");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(12);
        table.Columns.Should().Contain(c => c.ColumnName == "task_name");
        table.Columns.Should().Contain(c => c.ColumnName == "task_description");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new TaskMap();

        var component = new Task
        {
            Name = "TaskA",
            Description = "This is a test task",
            Type = TaskType.Continuous,
            Priority = 10
        };

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["task_name"].Should().Be(component.Name);
    }

    [Test]
    public void GenerateTable_ManyElement_HasExpectedCount()
    {
        var map = new TaskMap();

        var table = map.GenerateTable(
        [
            new Task { Name = "TaskA", Description = "This is a test task", Type = TaskType.Continuous, Priority = 10 },
            new Task { Name = "TaskB", Description = "This is a test task", Type = TaskType.Periodic, Priority = 5 },
            new Task { Name = "TaskC", Description = "This is a test task", Type = TaskType.Event, Priority = 1 },
        ]);

        table.Rows.Should().HaveCount(3);
    }

    [Test]
    public void GenerateTable_SingleElement_ShouldHaveCorrectRecordHash()
    {
        var map = new TaskMap();
        var component = new Task { Name = "TaskA", Type = TaskType.Continuous, Priority = 10 };

        var table = map.GenerateTable([component]);

        var expectedHash = map.ComputeHash(component);
        table.Rows[0]["record_hash"].Should().Be(expectedHash);
    }

    [Test]
    public void ComputeHash_WithSameProperties_ShouldBeSame()
    {
        var map = new TaskMap();

        var first = new Task { Name = "TaskA", Type = TaskType.Periodic, Priority = 5, Rate = 100 };
        var second = new Task { Name = "TaskA", Type = TaskType.Periodic, Priority = 5, Rate = 100 };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_WithDifferentProperties_ShouldBeDifferent()
    {
        var map = new TaskMap();

        var first = new Task { Name = "TaskA", Type = TaskType.Periodic, Priority = 5, Rate = 100 };
        var second = new Task { Name = "TaskA", Type = TaskType.Periodic, Priority = 10, Rate = 100 };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }

    [Test]
    public void ComputeHash_PropertyChanged_ShouldChangeRecordHash()
    {
        var map = new TaskMap();
        var component = new Task { Name = "TaskA", Type = TaskType.Continuous, Priority = 10 };
        var initialHash = map.ComputeHash(component);

        component.Metadata.Clear(); // Clear cache
        component.Priority = 5;
        var newHash = map.ComputeHash(component);

        newHash.Should().NotBe(initialHash);
    }

    [Test]
    public void ComputeHash_NewPropertiesShowsUp_ShouldBeSameTaskDoesNotHaveContentHash()
    {
        var map = new TaskMap();

        var first = new Task
        {
            Name = "TaskA",
            Description = "This is a test task",
            Type = TaskType.Continuous,
            Priority = 10
        };

        var second = new Task
        {
            Name = "TaskA",
            Description = "This is a test task",
            Type = TaskType.Continuous,
            Priority = 10
        };

        second.Serialize().SetAttributeValue("NewProperty", "123");

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }
}