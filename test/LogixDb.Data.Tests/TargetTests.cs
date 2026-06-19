using System.Diagnostics;
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

        var target = new Target
        {
            TargetType = "Controller",
            TargetName = "TestController",
            IsPartial = false,
            ImportDate = DateTime.UtcNow,
            ImportUser = "TestUser",
            ImportMachine = "TestMachine",
            SourceHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(sourceData)),
            SourceData = sourceData
        };

        target.TargetType.Should().Be("Controller");
        target.TargetName.Should().Be("TestController");
        target.IsPartial.Should().BeFalse();
        target.SourceHash.Should().NotBeEmpty();
        target.SourceData.Should().NotBeEmpty();
    }

    [Test]
    public void Create_FromFakeSource_ShouldHaveExpectedFields()
    {
        var source = TestSource.LocalTest();

        var target = Target.Create(source, "TestProject");

        target.Should().NotBeNull();
        target.TargetKey.Should().Be($"{source.Content.TargetType?.ToLower()}://{source.Content.TargetName}");
        target.TargetType.Should().Be(source.Content.TargetType);
        target.TargetName.Should().Be(source.Content.TargetName);
        target.IsPartial.Should().Be(source.Content.ContainsContext);
        target.SourceHash.Should().NotBeEmpty();
        target.SourceData.Should().NotBeEmpty();
    }

    [Test]
    public void Create_WithTargetKey_ShouldUseProvidedKey()
    {
        var source = TestSource.LocalTest();
        const string customKey = "custom://mykey";

        var target = Target.Create(source, customKey);

        target.TargetKey.Should().Be(customKey);
    }

    [Test]
    public void GetSource_WhenCalled_ShouldReturnParsedL5X()
    {
        var source = TestSource.LocalTest();
        var target = Target.Create(source, "TestProject");

        var retrievedSource = target.GetSource();

        retrievedSource.Should().NotBeNull();
        retrievedSource.Content.TargetName.Should().Be(source.Content.TargetName);
    }

    [Test]
    public void ToString_WhenCalled_ShouldReturnTargetKey()
    {
        var source = TestSource.LocalTest();
        var target = Target.Create(source, "TestProject");

        target.ToString().Should().Be(target.TargetKey);
    }

    [Test]
    public void Compile_ExplicitValidTables_ShouldReturnExpectedTables()
    {
        var source = TestSource.LocalTest();
        var target = Target.Create(source, "TestProject");

        var tables = target.Compile().ToList();

        tables.Should().NotBeEmpty();
        tables.Should().Contain(t => t.TableName == "controller");
        tables.Should().Contain(t => t.TableName == "tag");
    }

    [Test]
    public void Compile_AllTablesAgainstLocalTests_ShouldBePerformant()
    {
        var source = TestSource.LocalExample();
        var target = Target.Create(source, "TestProject");

        var stopwatch = Stopwatch.StartNew();
        var tables = target.Compile().ToList();
        stopwatch.Stop();

        tables.Should().NotBeEmpty();
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(3));
        Console.Write(stopwatch.ElapsedMilliseconds);
    }
}