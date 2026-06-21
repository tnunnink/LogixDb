using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Tests;

[TestFixture]
public class CryptoExtensionsTests
{
    [Test]
    public void HashText_SameText_ShouldBeSame()
    {
        var text1 = "Hello World";
        var text2 = "Hello World";

        text1.HashText().Should().Be(text2.HashText());
    }

    [Test]
    public void HashText_DifferentText_ShouldBeDifferent()
    {
        var text1 = "Hello World";
        var text2 = "hello world";

        text1.HashText().Should().NotBe(text2.HashText());
    }

    [Test]
    public void HashElement_SameElement_ShouldBeSame()
    {
        var tag1 = new Tag { Name = "Tag1", Value = 123 };
        var tag2 = new Tag { Name = "Tag1", Value = 456 }; // Value should be scrubbed

        var hash1 = tag1.HashElement();
        var hash2 = tag2.HashElement();

        hash1.Should().Be(hash2);
    }

    [Test]
    public void HashElement_DifferentElement_ShouldBeDifferent()
    {
        var tag1 = new Tag { Name = "Tag1" };
        var tag2 = new Tag { Name = "Tag2" };

        var hash1 = tag1.HashElement();
        var hash2 = tag2.HashElement();

        hash1.Should().NotBe(hash2);
    }

    [Test]
    public void HashElement_WithCaches_ShouldUseCache()
    {
        var tag = new Tag { Name = "Tag1" };
        var hash1 = tag.HashElement();
        
        tag.Metadata["content_hash"].Should().Be(hash1);
        
        tag.Metadata["content_hash"] = "fake_hash";
        tag.HashElement().Should().Be("fake_hash");
    }
}
