using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class AoiMapTests
{
    [Test]
    public void GenerateTable_BasicAoi_ShouldMapAllProperties()
    {
        var map = new AoiMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("aoi");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(21);
        table.Columns.Should().Contain(c => c.ColumnName == "aoi_name");
        table.Columns.Should().Contain(c => c.ColumnName == "aoi_description");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new AoiMap();
        var component = new AddOnInstruction { Name = "AoiA", Description = "Test AOI" };

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["aoi_name"].Should().Be(component.Name);
    }

    [Test]
    public void ComputeHash_SameAois_ShouldBeSame()
    {
        var map = new AoiMap();

        var first = new AddOnInstruction
        {
            Name = "AoiA",
            Description = "Test",
            CreatedDate = DateTime.Today,
            EditedDate = DateTime.Today
        };

        var second = new AddOnInstruction
        {
            Name = "AoiA",
            Description = "Test",
            CreatedDate = DateTime.Today,
            EditedDate = DateTime.Today
        };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_DifferentAois_ShouldBeDifferent()
    {
        var map = new AoiMap();

        var first = new AddOnInstruction { Name = "AoiA", Description = "Test1" };
        var second = new AddOnInstruction { Name = "AoiA", Description = "Test2" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }
}