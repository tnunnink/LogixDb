using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class DataTypeMemberMapTests
{
    [Test]
    public void GenerateTable_BasicDataTypeMember_ShouldMapAllProperties()
    {
        var map = new DataTypeMemberMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("data_type_member");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(12);
        table.Columns.Should().Contain(c => c.ColumnName == "type_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "member_name");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new DataTypeMemberMap();
        var component = new DataTypeMember { Name = "MemberA", DataType = "DINT" };
        component.Metadata.Add("member_index", 0);

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["member_name"].Should().Be(component.Name);
    }
    
    [Test]
    public void GenerateTable_WithParent_ShouldMapTypeHash()
    {
        var map = new DataTypeMemberMap();
        var parent = new DataType("TypeA");
        parent.Metadata.Add("record_hash", "ABC");
        
        parent.AddMember("MemberA", "DINT");
        var component = parent.Members.First();
        component.Metadata.Add("member_index", 0);

        var table = map.GenerateTable([component]);

        table.Rows[0]["type_hash"].Should().Be("ABC");
    }

    [Test]
    public void ComputeHash_SameMembers_ShouldBeSame()
    {
        var map = new DataTypeMemberMap();

        var first = new DataTypeMember { Name = "MemberA", Description = "Test" };
        first.Metadata.Add("member_index", 0);
        var second = new DataTypeMember { Name = "MemberA", Description = "Test" };
        second.Metadata.Add("member_index", 0);

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_DifferentMembers_ShouldBeDifferent()
    {
        var map = new DataTypeMemberMap();

        var first = new DataTypeMember { Name = "MemberA", Description = "Test1" };
        first.Metadata.Add("member_index", 0);
        var second = new DataTypeMember { Name = "MemberA", Description = "Test2" };
        second.Metadata.Add("member_index", 0);

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }
}
