using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Extensions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Tests.Maps;

[TestFixture]
public class DataTypeMapTests
{
    [Test]
    public void GenerateTable_BasicDataType_ShouldMapAllProperties()
    {
        var map = new DataTypeMap();

        var table = map.GenerateTable([]);

        table.TableName.Should().Be("data_type");
        table.Rows.Should().BeEmpty();
        table.Columns.Should().HaveCount(6);
        table.Columns.Should().Contain(c => c.ColumnName == "type_name");
        table.Columns.Should().Contain(c => c.ColumnName == "type_description");
        table.Columns.Should().Contain(c => c.ColumnName == "type_class");
        table.Columns.Should().Contain(c => c.ColumnName == "type_family");
        table.Columns.Should().Contain(c => c.ColumnName == "content_hash");
        table.Columns.Should().Contain(c => c.ColumnName == "record_hash");
    }

    [Test]
    public void GenerateTable_SingleElement_HasExpectedCount()
    {
        var map = new DataTypeMap();
        var component = new DataType("TypeA") { Description = "This is a test type" };

        var table = map.GenerateTable([component]);

        table.Rows.Should().HaveCount(1);
        table.Rows[0]["type_name"].Should().Be(component.Name);
    }

    [Test]
    public void GenerateTable_ManyElement_HasExpectedCount()
    {
        var map = new DataTypeMap();

        var table = map.GenerateTable(
        [
            new DataType("TypeA") { Description = "This is a test type" },
            new DataType("TypeB") { Description = "This is a test type" },
            new DataType("TypeC") { Description = "This is a test type" },
        ]);

        table.Rows.Should().HaveCount(3);
    }

    [Test]
    public void GenerateTable_SingleElement_ShouldHaveCorrectRecordHash()
    {
        var map = new DataTypeMap();
        var component = new DataType("TypeA");

        var table = map.GenerateTable([component]);

        var expectedHash = map.ComputeHash(component);
        table.Rows[0]["record_hash"].Should().Be(expectedHash);
    }

    [Test]
    public void ComputeHash_SameTypes_ShouldBeSame()
    {
        var map = new DataTypeMap();

        var first = new DataType("TypeA") { Description = "This is a test type" };
        var second = new DataType("TypeA") { Description = "This is a test type" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_SlightlyDifferentDescription_ShouldBeDifferent()
    {
        var map = new DataTypeMap();

        var first = new DataType("TypeA") { Description = "This is a test  type" };
        var second = new DataType("TypeA") { Description = "This is a test type" };

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }

    [Test]
    public void ComputeHash_WithSameMembers_ShouldBeSame()
    {
        var map = new DataTypeMap();

        var first = new DataType("TypeA").AddMember("Field1", "DINT").AddMember("Property1", "TIMER");
        var second = new DataType("TypeA").AddMember("Field1", "DINT").AddMember("Property1", "TIMER");

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().Be(secondHash);
    }

    [Test]
    public void ComputeHash_WithDifferentMembers_ShouldBeSame()
    {
        var map = new DataTypeMap();

        var first = new DataType("TypeA").AddMember("Field1", "DINT").AddMember("Property1", "TIMER");
        var second = new DataType("TypeA").AddMember("Field1", "BOOL").AddMember("Property1", "TIMER");

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }

    [Test]
    public void ComputeHash_MemberAdded_ShouldChangeRecordHash()
    {
        var map = new DataTypeMap();
        var component = new DataType("TypeA");
        var initialHash = map.ComputeHash(component);

        component.Metadata.Clear(); // Clear cache
        component.AddMember("NewField", "DINT");
        var newHash = map.ComputeHash(component);

        newHash.Should().NotBe(initialHash);
    }

    [Test]
    public void ComputeHash_NewPropertiesShowsUp_ShouldBeDifferentBecauseWeComputeElementHash()
    {
        var map = new DataTypeMap();

        var first = new DataType("TypeA") { Description = "This is a test  type" };
        var second = new DataType("TypeA") { Description = "This is a test type" };
        second.Serialize().SetAttributeValue("NewProperty", "123");

        var firstHash = map.ComputeHash(first);
        var secondHash = map.ComputeHash(second);

        firstHash.Should().NotBe(secondHash);
    }

    [Test]
    public void ComputeHash_BetweenCurrentAndFakeNewMapWithNewProperty_ShouldBeSameHashSinceWeOmitNulls()
    {
        var currentMap = new DataTypeMap();
        var newMap = new DataTypeMapAddedField();

        var component = new DataType("TypeA") { Description = "This is a test  type" };

        var currentHash = currentMap.ComputeHash(component);
        //we have to clear that hash because the code is caching it
        component.Metadata.Clear();
        var newHash = newMap.ComputeHash(component);

        currentHash.Should().Be(newHash);
    }
}

/// <summary>
/// This is just to test what could happen to the hash if we update the map to include new properties.
/// If theory we should retain the same hash because we omit nulls. 
/// </summary>
public class DataTypeMapAddedField : TableMap<DataType>
{
    /// <inheritdoc />
    protected override string TableName => "data_type";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<DataType>> Columns =>
    [
        ColumnMap<DataType>.For(r => r.Name, "type_name"),
        ColumnMap<DataType>.For(r => r.Description, "type_description"),
        ColumnMap<DataType>.For(r => r.Class.Name, "type_class"),
        ColumnMap<DataType>.For(r => r.Family.Name, "type_family"),
        ColumnMap<DataType>.For(r => r.Serialize().Attribute("new_field")?.Value, "new_field"),
        ColumnMap<DataType>.For(r => r.HashElement(), "content_hash"),
        ColumnMap<DataType>.RecordHash(this)
    ];
}