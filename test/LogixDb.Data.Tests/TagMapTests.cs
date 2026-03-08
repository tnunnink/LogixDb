using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class TagMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new TagMap();
        List<Tag> tags = [new("First", 1), new("Second", 2), new("Third", 3)];
        var records = tags.Select(t => new TagRecord(1, t));

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(3);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new TagMap();
        var tag = new Tag("TestTag", 100) { Description = "Test Description" };
        var records = new List<TagRecord> { new(1, tag) };

        var table = map.GenerateTable(records);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["tag_name"].Should().Be("TestTag");
        table.Rows[0]["tag_value"].Should().Be("100");
        table.Rows[0]["data_type"].Should().Be("DINT");
        table.Rows[0]["description"].Should().Be("Test Description");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new TagMap();
        var tag = new Tag("TestTag", 100);
        var record = new TagRecord(1, tag);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }

    [Test]
    public void ComputeHash_WithDifferentRecord_ShouldNotBeEqual()
    {
        var map = new TagMap();
        var record1 = new TagRecord(1, new Tag("Tag1", 1));
        var record2 = new TagRecord(1, new Tag("Tag2", 2));

        var hash1 = map.ComputeHash(record1);
        var hash2 = map.ComputeHash(record2);

        hash1.Should().NotBeEquivalentTo(hash2);
    }
}