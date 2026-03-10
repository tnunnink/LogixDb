using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class RoutineMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new RoutineMap();
        var records = new List<RoutineRecord>
        {
            new(1, new Routine { Name = "Routine1" }),
            new(1, new Routine { Name = "Routine2" })
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new RoutineMap();
        var routine = new Routine { Name = "TestRoutine", Description = "Test Description" };
        var record = new RoutineRecord(1, routine);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["routine_name"].Should().Be("TestRoutine");
        table.Rows[0]["routine_description"].Should().Be("Test Description");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new RoutineMap();
        var routine = new Routine { Name = "TestRoutine" };
        var record = new RoutineRecord(1, routine);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}
