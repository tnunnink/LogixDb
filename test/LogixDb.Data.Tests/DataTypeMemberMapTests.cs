using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class DataTypeMemberMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new DataTypeMemberMap();
        var dt = new DataType("TestType");
        dt.Members.Add(new DataTypeMember { Name = "Member1", DataType = "INT" });
        dt.Members.Add(new DataTypeMember { Name = "Member2", DataType = "DINT" });
        var records = dt.Members.Select(m => new DataTypeMemberRecord(1, m)).ToList();

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new DataTypeMemberMap();
        var dt = new DataType("TestType");
        var member = new DataTypeMember { Name = "TestMember", DataType = "INT" };
        member.Description = "Test Description";
        dt.Members.Add(member);
        var record = new DataTypeMemberRecord(1, member);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["type_name"].Should().Be("TestType");
        table.Rows[0]["member_name"].Should().Be("TestMember");
        table.Rows[0]["data_type"].Should().Be("INT");
        table.Rows[0]["member_description"].Should().Be("Test Description");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new DataTypeMemberMap();
        var dt = new DataType("TestType");
        var member = new DataTypeMember { Name = "TestMember", DataType = "INT" };
        dt.Members.Add(member);
        var record = new DataTypeMemberRecord(1, member);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}
