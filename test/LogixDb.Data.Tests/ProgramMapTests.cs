using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class ProgramMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new ProgramMap();
        var records = new List<ProgramRecord>
        {
            new(1, new Program { Name = "Prog1" }),
            new(1, new Program { Name = "Prog2" })
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new ProgramMap();
        var program = new Program { Name = "TestProg", Description = "Test Description" };
        var record = new ProgramRecord(1, program);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["program_name"].Should().Be("TestProg");
        table.Rows[0]["program_description"].Should().Be("Test Description");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new ProgramMap();
        var program = new Program { Name = "TestProg" };
        var record = new ProgramRecord(1, program);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}
