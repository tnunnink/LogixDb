using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class TagMapTests
{
    [Test]
    public void GenerateTable_BasicTag_ShouldMapAllProperties()
    {
        var map = new TagMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("tag");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(14);
        table.Columns.Should().Contain(c => c.ColumnName == "tag_name");
        table.Columns.Should().Contain(c => c.ColumnName == "tag_description");
        table.Columns.Should().Contain(c => c.ColumnName == "content_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new TagMap();
        var component = new Tag { Name = "TagA", Value = 123, Description = "Test Tag" };

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["tag_name"].Should().Be(component.Name);
    }

    [Test]
    public void ComputeHash_SameTags_ShouldBeSame()
    {
        var map = new TagMap();

        var first = new Tag { Name = "TagA", Value = 123, Description = "Test" };
        var second = new Tag { Name = "TagA", Value = 123, Description = "Test" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_SameTagsDifferentValues_ShouldBeSame()
    {
        var map = new TagMap();

        var first = new Tag { Name = "TagA", Value = 123, Description = "Test" };
        var second = new Tag { Name = "TagA", Value = 124, Description = "Test" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_SameTagsComplexTypesDifferentValues_ShouldBeSame()
    {
        var map = new TagMap();

        var first = new Tag { Name = "TagA", Value = new TIMER { PRE = 1234 }, Description = "Test" };
        var second = new Tag { Name = "TagA", Value = new TIMER { PRE = 4567 }, Description = "Test" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_DifferentTags_ShouldBeDifferent()
    {
        var map = new TagMap();

        var first = new Tag { Name = "TagA", Description = "Test1" };
        var second = new Tag { Name = "TagA", Description = "Test2" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }
}