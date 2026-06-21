using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class RoutineMapTests
{
    [Test]
    public void GenerateTable_BasicRoutine_ShouldMapAllProperties()
    {
        var map = new RoutineMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("routine");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(6);
        table.Columns.Should().Contain(c => c.ColumnName == "routine_name");
        table.Columns.Should().Contain(c => c.ColumnName == "routine_description");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new RoutineMap();
        var component = new Routine { Name = "RoutineA", Description = "Test Routine" };

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["routine_name"].Should().Be(component.Name);
    }

    [Test]
    public void ComputeHash_SameRoutines_ShouldBeSame()
    {
        var map = new RoutineMap();

        var first = new Routine { Name = "RoutineA", Description = "Test" };
        var second = new Routine { Name = "RoutineA", Description = "Test" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_DifferentRoutines_ShouldBeDifferent()
    {
        var map = new RoutineMap();

        var first = new Routine { Name = "RoutineA", Description = "Test1" };
        var second = new Routine { Name = "RoutineA", Description = "Test2" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }
}
