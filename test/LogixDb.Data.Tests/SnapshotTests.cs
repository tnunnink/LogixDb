using FluentAssertions;
using LogixDb.Testing;

namespace LogixDb.Data.Tests;

[TestFixture]
public class SnapshotTests
{
    [Test]
    public void Snapshot_CanBeCreated_WithRequiredFields()
    {
        var sourceData = new byte[] { 1, 2, 3, 4, 5 };
        var snapshot = new Snapshot
        {
            SnapshotId = 1,
            TargetType = "Controller",
            TargetName = "TestController",
            IsPartial = false,
            ImportDate = DateTime.UtcNow,
            ImportUser = "TestUser",
            ImportMachine = "TestMachine",
            SourceHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(sourceData)),
            SourceData = sourceData
        };

        snapshot.SnapshotId.Should().Be(1);
        snapshot.TargetType.Should().Be("Controller");
        snapshot.TargetName.Should().Be("TestController");
        snapshot.IsPartial.Should().BeFalse();
        snapshot.SourceHash.Should().NotBeEmpty();
        snapshot.SourceData.Should().NotBeEmpty();
    }

    [Test]
    public void Snapshot_CreateFromFakeSource_ShouldHaveExpectedFields()
    {
        var source = TestSource.LocalTest();

        var snapshot = Snapshot.Create(source);

        snapshot.Should().NotBeNull();
        snapshot.TargetKey.Should().Be($"{source.Content.TargetType?.ToLower()}://{source.Content.TargetName}");
        snapshot.TargetType.Should().Be(source.Content.TargetType);
        snapshot.TargetName.Should().Be(source.Content.TargetName);
        snapshot.IsPartial.Should().Be(source.Content.ContainsContext);
        snapshot.SourceHash.Should().NotBeEmpty();
        snapshot.SourceData.Should().NotBeEmpty();
    }

    [Test]
    public void Snapshot_GetSource_ShouldReturnParsedL5X()
    {
        var source = TestSource.LocalTest();
        var snapshot = Snapshot.Create(source);

        var retrievedSource = snapshot.GetSource();

        retrievedSource.Should().NotBeNull();
        retrievedSource.Content.TargetName.Should().Be(source.Content.TargetName);
    }

    [Test]
    public void Snapshot_ToString_ShouldReturnTargetKey()
    {
        var source = TestSource.LocalTest();
        var snapshot = Snapshot.Create(source);

        snapshot.ToString().Should().Be(snapshot.TargetKey);
    }

    [Test]
    public void Snapshot_Compile_ShouldReturnExpectedTables()
    {
        var source = TestSource.LocalTest();
        var snapshot = Snapshot.Create(source);
        var tableNames = new List<string> { "controller", "data_type" };

        var tables = snapshot.Compile(tableNames).ToList();

        tables.Should().NotBeEmpty();
        tables.Should().Contain(t => t.TableName == "controller");
        // The fake source might not have data_types, but it should definitely have a controller.
    }
}