using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class RungMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new RungMap();
        var records = new List<RungRecord>
        {
            new(1, new Rung("XIC(Tag1)OTE(Tag2);")),
            new(1, new Rung("XIC(Tag3)OTE(Tag4);"))
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new RungMap();
        var rung = new Rung("XIC(Tag1)OTE(Tag2);") { Comment = "Test Comment" };
        var record = new RungRecord(1, rung);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["comment"].Should().Be("Test Comment");
        table.Rows[0]["code"].Should().Be("XIC(Tag1)OTE(Tag2);");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new RungMap();
        var rung = new Rung("XIC(Tag1)OTE(Tag2);");
        var record = new RungRecord(1, rung);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}
