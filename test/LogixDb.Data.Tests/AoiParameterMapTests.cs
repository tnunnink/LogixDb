using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class AoiParameterMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new AoiParameterMap();
        var records = new List<AoiParameterRecord>
        {
            new(1, "ParentAoi", new Parameter { Name = "Param1" }),
            new(1, "ParentAoi", new Parameter { Name = "Param2" })
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new AoiParameterMap();
        var parameter = new Parameter
        {
            Name = "TestParam",
            Description = "Test Description"
        };
        var record = new AoiParameterRecord(1, "ParentAoi", parameter);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["parameter_name"].Should().Be("TestParam");
        table.Rows[0]["parameter_description"].Should().Be("Test Description");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new AoiParameterMap();
        var parameter = new Parameter { Name = "TestParam" };
        var record = new AoiParameterRecord(1, "ParentAoi", parameter);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}