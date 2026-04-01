using FluentAssertions;

namespace LogixDb.Data.Tests;

[TestFixture]
public class MigrationTagTests
{
    [Test]
    public void GetTags_NullOptions_ShouldReturnRequiredAndComponent()
    {
        var tags = MigrationTag.GetTags(null).ToList();

        tags.Should().Contain(MigrationTag.Required);
        tags.Should().Contain(MigrationTag.Component);
        tags.Count.Should().Be(2);
    }

    [Test]
    public void GetTags_EmptyOptions_ShouldReturnRequiredAndComponent()
    {
        var options = new DbOptions { Include = [], Exclude = [] };
        var tags = MigrationTag.GetTags(options).ToList();

        tags.Should().Contain(MigrationTag.Required);
        tags.Should().Contain(MigrationTag.Component);
        tags.Count.Should().Be(2);
    }

    [Test]
    public void GetTags_IncludeController_ShouldReturnRequiredAndController()
    {
        var options = new DbOptions { Include = [MigrationTag.Controller] };
        var tags = MigrationTag.GetTags(options).ToList();

        tags.Should().Contain(MigrationTag.Required);
        tags.Should().Contain(MigrationTag.Controller);
        tags.Should().NotContain(MigrationTag.Component);
    }

    [Test]
    public void GetTags_ExcludeTag_ShouldReturnRequiredAndOthers()
    {
        var options = new DbOptions { Exclude = [MigrationTag.Tag] };
        var tags = MigrationTag.GetTags(options).ToList();

        tags.Should().Contain(MigrationTag.Required);
        tags.Should().NotContain(MigrationTag.Tag);
        tags.Should().Contain(MigrationTag.Controller);
        tags.Should().Contain(MigrationTag.Aoi);
    }
}
