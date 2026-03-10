using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class ControllerMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new ControllerMap();
        var records = new List<ControllerRecord>
        {
            new(1, new Controller { Name = "Ctrl1" }),
            new(1, new Controller { Name = "Ctrl2" })
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new ControllerMap();
        var controller = new Controller { Name = "TestCtrl" };
        controller.ProcessorType = "Type1";
        controller.Revision = new Revision(1, 1);
        var record = new ControllerRecord(1, controller);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["controller_name"].Should().Be("TestCtrl");
        table.Rows[0]["catalog_number"].Should().Be("Type1");
        table.Rows[0]["controller_revision"].Should().Be("1.1");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new ControllerMap();
        var controller = new Controller { Name = "TestCtrl" };
        var record = new ControllerRecord(1, controller);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}
