using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class ModuleMapTests
{
    [Test]
    public void GenerateTable_BasicModule_ShouldMapAllProperties()
    {
        var map = new ModuleMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("module");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(18);
        table.Columns.Should().Contain(c => c.ColumnName == "module_name");
        table.Columns.Should().Contain(c => c.ColumnName == "catalog_number");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new ModuleMap();
        var component = new Module { Name = "ModuleA", CatalogNumber = "1756-L83E" };

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["module_name"].Should().Be(component.Name);
    }

    [Test]
    public void ComputeHash_SameModules_ShouldBeSame()
    {
        var map = new ModuleMap();

        var first = new Module { Name = "ModuleA", Description = "Test" };
        var second = new Module { Name = "ModuleA", Description = "Test" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_DifferentModules_ShouldBeDifferent()
    {
        var map = new ModuleMap();

        var first = new Module { Name = "ModuleA", Description = "Test1" };
        var second = new Module { Name = "ModuleA", Description = "Test2" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }
}
