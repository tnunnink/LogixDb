using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class DataTypeMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new DataTypeMap();
        var records = new List<DataTypeRecord>
        {
            new(1, new DataType("Type1")),
            new(1, new DataType("Type2"))
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new DataTypeMap();
        var dt = new DataType("TestType") { Description = "Test Description" };
        var record = new DataTypeRecord(1, dt);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["type_name"].Should().Be("TestType");
        table.Rows[0]["type_description"].Should().Be("Test Description");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new DataTypeMap();
        var dt = new DataType("TestType");
        var record = new DataTypeRecord(1, dt);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}
