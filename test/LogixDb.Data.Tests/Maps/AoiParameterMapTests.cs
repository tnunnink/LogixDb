using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class AoiParameterMapTests
{
    [Test]
    public void GenerateTable_BasicParameter_ShouldMapAllProperties()
    {
        var map = new AoiParameterMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("aoi_parameter");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(15);
        table.Columns.Should().Contain(c => c.ColumnName == "aoi_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "parameter_name");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new AoiParameterMap();
        var component = new Parameter { Name = "ParamA", DataType = "DINT" };

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["parameter_name"].Should().Be(component.Name);
    }
    
    [Test]
    public void GenerateTable_WithParent_ShouldMapAoiHash()
    {
        var map = new AoiParameterMap();
        var parent = new AddOnInstruction { Name = "AoiA" };
        parent.Metadata.Add("record_hash", "ABC");
        
        var component = new Parameter { Name = "ParamA", DataType = "DINT" };
        parent.Parameters.Add(component);

        var table = map.GenerateTable([component]);

        table.Rows[0]["aoi_hash"].Should().Be("ABC");
    }

    [Test]
    public void ComputeHash_SameParameters_ShouldBeSame()
    {
        var map = new AoiParameterMap();

        var first = new Parameter { Name = "ParamA", Description = "Test" };
        var second = new Parameter { Name = "ParamA", Description = "Test" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_DifferentParameters_ShouldBeDifferent()
    {
        var map = new AoiParameterMap();

        var first = new Parameter { Name = "ParamA", Description = "Test1" };
        var second = new Parameter { Name = "ParamA", Description = "Test2" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }
}
