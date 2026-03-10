using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class AoiMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new AoiMap();
        var records = new List<AoiRecord>
        {
            new(1, new AddOnInstruction { Name = "AOI1" }),
            new(1, new AddOnInstruction { Name = "AOI2" })
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new AoiMap();
        var aoi = new AddOnInstruction
        {
            Name = "TestAOI",
            Revision = new Revision(1, 2),
            Description = "Test Description",
            Vendor = "Test Vendor"
        };
        var records = new List<AoiRecord> { new(1, aoi) };

        var table = map.GenerateTable(records);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["aoi_name"].Should().Be("TestAOI");
        table.Rows[0]["aoi_revision"].Should().Be("1.2");
        table.Rows[0]["aoi_description"].Should().Be("Test Description");
        table.Rows[0]["aoi_vendor"].Should().Be("Test Vendor");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new AoiMap();
        var aoi = new AddOnInstruction { Name = "TestAOI" };
        var record = new AoiRecord(1, aoi);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }

    [Test]
    public void ComputeHash_WithDifferentRecord_ShouldNotBeEqual()
    {
        var map = new AoiMap();
        var record1 = new AoiRecord(1, new AddOnInstruction { Name = "AOI1" });
        var record2 = new AoiRecord(1, new AddOnInstruction { Name = "AOI2" });

        var hash1 = map.ComputeHash(record1);
        var hash2 = map.ComputeHash(record2);

        hash1.Should().NotBeEquivalentTo(hash2);
    }
}
