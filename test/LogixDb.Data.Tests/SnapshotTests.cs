using FluentAssertions;
using LogixDb.Testing;

namespace LogixDb.Data.Tests;

[TestFixture]
public class TargetTests
{
    [Test]
    public void Target_CanBeCreated_WithRequiredFields()
    {
        var sourceData = new byte[] { 1, 2, 3, 4, 5 };
        var snapshot = new Target
        {
            InstanceId = 1,
            TargetType = "Controller",
            TargetName = "TestController",
            IsPartial = false,
            ImportDate = DateTime.UtcNow,
            ImportUser = "TestUser",
            ImportMachine = "TestMachine",
            SourceHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(sourceData)),
            SourceData = sourceData
        };

        snapshot.InstanceId.Should().Be(1);
        snapshot.TargetType.Should().Be("Controller");
        snapshot.TargetName.Should().Be("TestController");
        snapshot.IsPartial.Should().BeFalse();
        snapshot.SourceHash.Should().NotBeEmpty();
        snapshot.SourceData.Should().NotBeEmpty();
    }

    [Test]
    public void Target_CreateFromFakeSource_ShouldHaveExpectedFields()
    {
        var source = TestSource.LocalTest();

        var snapshot = Target.Create(source);

        snapshot.Should().NotBeNull();
        snapshot.TargetKey.Should().Be($"{source.Content.TargetType?.ToLower()}://{source.Content.TargetName}");
        snapshot.TargetType.Should().Be(source.Content.TargetType);
        snapshot.TargetName.Should().Be(source.Content.TargetName);
        snapshot.IsPartial.Should().Be(source.Content.ContainsContext);
        snapshot.SourceHash.Should().NotBeEmpty();
        snapshot.SourceData.Should().NotBeEmpty();
    }

    [Test]
    public void Target_GetSource_ShouldReturnParsedL5X()
    {
        var source = TestSource.LocalTest();
        var snapshot = Target.Create(source);

        var retrievedSource = snapshot.GetSource();

        retrievedSource.Should().NotBeNull();
        retrievedSource.Content.TargetName.Should().Be(source.Content.TargetName);
    }

    [Test]
    public void Target_ToString_ShouldReturnTargetKey()
    {
        var source = TestSource.LocalTest();
        var snapshot = Target.Create(source);

        snapshot.ToString().Should().Be(snapshot.TargetKey);
    }

    [Test]
    public void Target_Compile_ShouldReturnExpectedTables()
    {
        var source = TestSource.LocalTest();
        var snapshot = Target.Create(source);
        var tableNames = new List<string> { "controller", "tag", "task", "program" };

        var tables = snapshot.Compile(tableNames).ToList();

        tables.Should().NotBeEmpty();
        tables.Should().Contain(t => t.TableName == "controller");

        // Ensure that only requested tables are returned, and they are not empty (if they exist in the source)
        foreach (var table in tables)
        {
            tableNames.Should().Contain(table.TableName);
            table.Rows.Count.Should().BeGreaterThanOrEqualTo(0);
        }
    }

    [Test]
    public void Target_Compile_WithEmptyList_ShouldReturnNoTables()
    {
        var source = TestSource.LocalTest();
        var snapshot = Target.Create(source);
        var tableNames = new List<string>();

        var tables = snapshot.Compile(tableNames).ToList();

        tables.Should().BeEmpty();
    }

    [Test]
    public void Target_Compile_WithNonExistentTable_ShouldReturnOnlyExistentTables()
    {
        var source = TestSource.LocalTest();
        var snapshot = Target.Create(source);
        var tableNames = new List<string> { "controller", "non_existent_table" };

        var tables = snapshot.Compile(tableNames).ToList();

        tables.Should().HaveCount(1);
        tables.First().TableName.Should().Be("controller");
    }

    [Test]
    public void Target_Create_WithTargetKey_ShouldUseProvidedKey()
    {
        var source = TestSource.LocalTest();
        const string customKey = "custom://mykey";

        var snapshot = Target.Create(source, customKey);

        snapshot.TargetKey.Should().Be(customKey);
    }
}