using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class AoiLocalTagMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new AoiLocalTagMap();
        var records = new List<AoiLocalTagRecord>
        {
            new(1, new LocalTag { Name = "Tag1" }),
            new(1, new LocalTag { Name = "Tag2" })
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new AoiLocalTagMap();
        var tag = new LocalTag { Name = "TestTag" };
        tag.Description = "Test Description";
        var record = new AoiLocalTagRecord(1, tag);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["parameter_name"].Should().Be("TestTag");
        table.Rows[0]["description"].Should().Be("Test Description");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new AoiLocalTagMap();
        var tag = new LocalTag { Name = "TestTag" };
        var record = new AoiLocalTagRecord(1, tag);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}
