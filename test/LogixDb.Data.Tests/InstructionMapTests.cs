using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class InstructionMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new InstructionMap();
        var records = new List<InstructionRecord>
        {
            new(1, Guid.NewGuid(), 0, new Instruction("XIC(TestTag)")),
            new(1, Guid.NewGuid(), 1, new Instruction("OTE(TestTag)"))
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new InstructionMap();
        var instr = Instruction.Parse("XIC(TestTag)");
        var record = new InstructionRecord(1, Guid.NewGuid(), 0, instr);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["instruction_key"].Should().Be("XIC");
        table.Rows[0]["instruction_text"].Should().Be("XIC(TestTag)");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new InstructionMap();
        var instr = Instruction.Parse("XIC(TestTag)");
        var record = new InstructionRecord(1, Guid.NewGuid(), 0, instr);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}