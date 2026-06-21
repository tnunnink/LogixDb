using FluentAssertions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class RungChildMapTests
{
    [Test]
    public void GenerateTable_RungInstruction_ShouldMapAllProperties()
    {
        var map = new RungInstructionMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("rung_instruction");
        table.Columns.Should().HaveCount(7);
        table.Columns.Should().Contain(c => c.ColumnName == "rung_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "instruction_text");
    }

    [Test]
    public void GenerateTable_RungArgument_ShouldMapAllProperties()
    {
        var map = new RungArgumentMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("rung_argument");
        table.Columns.Should().HaveCount(6);
        table.Columns.Should().Contain(c => c.ColumnName == "rung_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "argument_text");
    }

    [Test]
    public void GenerateTable_RungReference_ShouldMapAllProperties()
    {
        var map = new RungReferenceMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("rung_reference");
        table.Columns.Should().HaveCount(4);
        table.Columns.Should().Contain(c => c.ColumnName == "rung_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "reference_name");
    }

    [Test]
    public void GenerateTable_InstructionRecord_ShouldMapCorrectly()
    {
        var map = new RungInstructionMap();
        var record = new InstructionRecord("RUNG_HASH", 0, "XIC(Tag)", "XIC", true, true);
        
        var table = map.GenerateTable([record]);

        table.Rows[0]["rung_hash"].Should().Be("RUNG_HASH");
        table.Rows[0]["instruction_text"].Should().Be("XIC(Tag)");
    }
}
