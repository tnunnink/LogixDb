using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class ModuleChildMapTests
{
    [Test]
    public void GenerateTable_ModulePort_ShouldMapAllProperties()
    {
        var map = new ModulePortMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("module_port");
        table.Columns.Should().HaveCount(7);
        table.Columns.Should().Contain(c => c.ColumnName == "module_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "port_number");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_ModuleConnection_ShouldMapAllProperties()
    {
        var map = new ModuleConnectionMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("module_connection");
        table.Columns.Should().HaveCount(19);
        table.Columns.Should().Contain(c => c.ColumnName == "module_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "connection_name");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_PortWithParent_ShouldMapModuleHash()
    {
        var map = new ModulePortMap();
        var parent = new Module { Name = "ModuleA" };
        parent.Metadata.Add("record_hash", "ABC");
        
        var component = new Port { Id = 1, Type = "Ethernet" };
        parent.Ports.Add(component);

        var table = map.GenerateTable([component]);

        table.Rows[0]["module_hash"].Should().Be("ABC");
    }

    [Test]
    public void GenerateTable_ConnectionWithParent_ShouldMapModuleHash()
    {
        var map = new ModuleConnectionMap();
        var parent = new Module { Name = "ModuleA" };
        parent.Metadata.Add("record_hash", "ABC");
        
        var component = new Connection { Name = "ConnA" };
        parent.Connections.Add(component);

        var table = map.GenerateTable([component]);

        table.Rows[0]["module_hash"].Should().Be("ABC");
    }
}
