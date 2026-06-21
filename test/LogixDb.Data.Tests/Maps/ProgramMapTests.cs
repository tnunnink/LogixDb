using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class ProgramMapTests
{
    [Test]
    public void GenerateTable_BasicProgram_ShouldMapAllProperties()
    {
        var map = new ProgramMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("program");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(11);
        table.Columns.Should().Contain(c => c.ColumnName == "program_name");
        table.Columns.Should().Contain(c => c.ColumnName == "program_description");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new ProgramMap();
        var component = new Program
        {
            Name = "ProgA",
            Description = "Test Program",
            Type = ProgramType.Normal
        };

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["program_name"].Should().Be(component.Name);
    }

    [Test]
    public void GenerateTable_ManyElement_HasExpectedCount()
    {
        var map = new ProgramMap();

        var table = map.GenerateTable(
        [
            new Program { Name = "ProgA" },
            new Program { Name = "ProgB" },
            new Program { Name = "ProgC" },
        ]);

        table.Rows.Should().HaveCount(3);
    }

    [Test]
    public void GenerateTable_SingleElement_ShouldHaveCorrectRecordHash()
    {
        var map = new ProgramMap();
        var component = new Program { Name = "ProgA" };

        var table = map.GenerateTable([component]);

        var expectedHash = map.ComputeHash(component);
        table.Rows[0]["record_hash"].Should().Be(expectedHash);
    }

    [Test]
    public void ComputeHash_SamePrograms_ShouldBeSame()
    {
        var map = new ProgramMap();

        var first = new Program { Name = "ProgA", Description = "Test" };
        var second = new Program { Name = "ProgA", Description = "Test" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_DifferentPrograms_ShouldBeDifferent()
    {
        var map = new ProgramMap();

        var first = new Program { Name = "ProgA", Description = "Test1" };
        var second = new Program { Name = "ProgA", Description = "Test2" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }
}
