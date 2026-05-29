using FluentAssertions;
using LogixDb.Testing;

namespace LogixDb.Data.Tests;

[TestFixture]
public class FakeSourceTests
{
    [Test]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(10000)]
    [TestCase(100000)]
    public void CreateFakeDataTypes_ProvidedCount_ShouldReturnExpectedCount(int count)
    {
        var types = TestSource.CreateFakeDataTypes(count);

        types.Should().HaveCount(count);
    }
    
    [Test]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(10000)]
    [TestCase(100000)]
    public void CreateFakeTags_ProvidedCount_ShouldReturnExpectedCount(int count)
    {
        var types = TestSource.CreateFakeTags(count);

        types.Should().HaveCount(count);
    }
    
    [Test]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(10000)]
    [TestCase(100000)]
    public void CreateFakeRoutines_ProvidedCount_ShouldReturnExpectedCount(int count)
    {
        var types = TestSource.CreateFakeRoutines(count);

        types.Should().HaveCount(count);
    }

    [Test]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(10000)]
    [TestCase(100000)]
    public void CreateFakeRungs_ProvidedCount_ShouldReturnExpectedCount(int count)
    {
        var fakes = TestSource.CreateFakeRungs(count);

        fakes.Should().HaveCount(count);
    }
}