using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests;

[TestFixture]
public class ModuleMapTests
{
    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedRowCount()
    {
        var map = new ModuleMap();
        var records = new List<ModuleRecord>
        {
            new(1, new Module { Name = "Mod1" }),
            new(1, new Module { Name = "Mod2" })
        };

        var table = map.GenerateTable(records);

        table.Rows.Count.Should().Be(2);
    }

    [Test]
    public void GenerateTable_WithValidRecords_ShouldHaveExpectedValues()
    {
        var map = new ModuleMap();
        var module = new Module { Name = "TestMod", CatalogNumber = "1756-L81E", Description = "Test Description" };
        var record = new ModuleRecord(1, module);

        var table = map.GenerateTable([record]);

        table.Rows[0]["snapshot_id"].Should().Be(1);
        table.Rows[0]["module_name"].Should().Be("TestMod");
        table.Rows[0]["catalog_number"].Should().Be("1756-L81E");
        table.Rows[0]["description"].Should().Be("Test Description");
    }

    [Test]
    public void ComputeHash_WithSameRecord_ShouldBeEqual()
    {
        var map = new ModuleMap();
        var module = new Module { Name = "TestMod" };
        var record = new ModuleRecord(1, module);

        var hash1 = map.ComputeHash(record);
        var hash2 = map.ComputeHash(record);

        hash1.Should().BeEquivalentTo(hash2);
    }
}
