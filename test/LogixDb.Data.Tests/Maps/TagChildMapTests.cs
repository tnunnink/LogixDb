using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class TagChildMapTests
{
    [Test]
    public void GenerateTable_TagMember_ShouldMapAllProperties()
    {
        var map = new TagMemberMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("tag_member");
        table.Columns.Should().HaveCount(5);
        table.Columns.Should().Contain(c => c.ColumnName == "tag_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "member_path");
    }

    [Test]
    public void GenerateTable_TagValue_ShouldMapAllProperties()
    {
        var map = new TagValueMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("tag_value");
        table.Columns.Should().HaveCount(3);
        table.Columns.Should().Contain(c => c.ColumnName == "tag_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "member_path");
        table.Columns.Should().Contain(c => c.ColumnName == "tag_value");
    }

    [Test]
    public void GenerateTable_TagMemberComment_ShouldMapAllProperties()
    {
        var map = new TagMemberCommentMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("tag_member_comment");
        table.Columns.Should().HaveCount(4);
        table.Columns.Should().Contain(c => c.ColumnName == "tag_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "member_path");
        table.Columns.Should().Contain(c => c.ColumnName == "comment");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_TagMember_WithBaseHash_ShouldMapCorrectly()
    {
        var map = new TagMemberMap();
        var tag = new Tag { Name = "MyTag" };
        // We can't set DataType directly if it's read-only, but usually Name/DataType are set in constructor or via L5X
        // If it's a mock or we just need the metadata on Base
        tag.Base.Metadata.Add("record_hash", "TAG_HASH");
        
        var table = map.GenerateTable([tag]);

        table.Rows[0]["tag_hash"].Should().Be("TAG_HASH");
    }

    [Test]
    public void GenerateTable_TagValueRecord_ShouldMapCorrectly()
    {
        var map = new TagValueMap();
        var record = new TagValueRecord("TAG_HASH", "Member.Path", "123");
        
        var table = map.GenerateTable([record]);

        table.Rows[0]["tag_hash"].Should().Be("TAG_HASH");
        table.Rows[0]["member_path"].Should().Be("Member.Path");
        table.Rows[0]["tag_value"].Should().Be("123");
    }

    [Test]
    public void GenerateTable_TagCommentRecord_ShouldMapCorrectly()
    {
        var map = new TagMemberCommentMap();
        var record = new TagCommentRecord("TAG_HASH", "Member.Path", "My Comment");
        
        var table = map.GenerateTable([record]);

        table.Rows[0]["tag_hash"].Should().Be("TAG_HASH");
        table.Rows[0]["member_path"].Should().Be("Member.Path");
        table.Rows[0]["comment"].Should().Be("My Comment");
    }
}
