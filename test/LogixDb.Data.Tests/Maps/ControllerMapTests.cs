using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class ControllerMapTests
{
    [Test]
    public void GenerateTable_BasicController_ShouldMapAllProperties()
    {
        var map = new ControllerMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("controller");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(33);
        table.Columns.Should().Contain(c => c.ColumnName == "controller_name");
        table.Columns.Should().Contain(c => c.ColumnName == "catalog_number");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new ControllerMap();
        var component = new Controller { Name = "ControllerA", ProcessorType = "1756-L83E" };

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["controller_name"].Should().Be(component.Name);
    }

    [Test]
    public void ComputeHash_SameControllers_ShouldBeSame()
    {
        var map = new ControllerMap();

        var first = new Controller { Name = "ControllerA", Description = "Test" };
        var second = new Controller { Name = "ControllerA", Description = "Test" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_DifferentControllers_ShouldBeDifferent()
    {
        var map = new ControllerMap();

        var first = new Controller { Name = "ControllerA", Description = "Test1" };
        var second = new Controller { Name = "ControllerA", Description = "Test2" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }
}
