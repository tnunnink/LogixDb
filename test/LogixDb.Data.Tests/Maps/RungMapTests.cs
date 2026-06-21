using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class RungMapTests
{
    [Test]
    public void GenerateTable_BasicRung_ShouldMapAllProperties()
    {
        var map = new RungMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("rung");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(7);
        table.Columns.Should().Contain(c => c.ColumnName == "rung_number");
        table.Columns.Should().Contain(c => c.ColumnName == "rung_text");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new RungMap();
        var component = new Rung { Number = 0, Text = "XIC(MyTag)OTE(MyOtherTag);", Comment = "Test Comment" };

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["rung_text"].Should().Be(component.Text);
    }

    [Test]
    public void ComputeHash_SameRungs_ShouldBeSame()
    {
        var map = new RungMap();

        var first = new Rung { Number = 0, Text = "XIC(MyTag)OTE(MyOtherTag);", Comment = "Test Comment" };
        var second = new Rung { Number = 0, Text = "XIC(MyTag)OTE(MyOtherTag);", Comment = "Test Comment" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_DifferentRungs_ShouldBeDifferent()
    {
        var map = new RungMap();

        var first = new Rung { Number = 0, Text = "XIC(MyTag)OTE(MyOtherTag);", Comment = "Test1" };
        var second = new Rung { Number = 0, Text = "XIC(MyTag)OTE(MyOtherTag);", Comment = "Test2" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }
}
