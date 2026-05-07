using FluentAssertions;
using LogixDb.Testing;

namespace LogixDb.Data.Tests;

[TestFixture]
public class ElementHasherTests
{
    [Test]
    public void Hash_ValidControllerElementFromTestFile_ShouldHaveExpectedOutput()
    {
        var controller = TestSource.LocalTest().Controller;

        var hash = ElementHasher.Hash(controller);

        hash.Should().Be("bf9eca182a71d5c0db603a037eed7055b81e65e738162ccf14bd98f6d8f10853");
    }
    
    
}