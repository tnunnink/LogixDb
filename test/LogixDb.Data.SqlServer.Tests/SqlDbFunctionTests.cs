using Dapper;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbFunctionTests : SqlServerTestFixture
{
    [TestCase("Tag", "Tag")]
    [TestCase("Tag.Member", "Tag")]
    [TestCase("Tag[0]", "Tag")]
    [TestCase("Tag.Member[0]", "Tag")]
    [TestCase("Tag[0].Member", "Tag")]
    public async Task TagBaseFunction_PostMigration_ShouldReturnExpectedBase(string tagName, string expected)
    {
        await using var connection = await Provider.OpenConnection();
        var result = await connection.ExecuteScalarAsync<string>("SELECT logix.tag_base(@tagName)", new { tagName });
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("Tag", "")]
    [TestCase("Tag.Member", "Member")]
    [TestCase("Tag[0]", "[0]")]
    [TestCase("Tag.Member[0]", "Member[0]")]
    [TestCase("Tag[0].Member", "[0].Member")]
    public async Task TagPathFunction_PostMigration_ShouldReturnExpectedPath(string tagName, string expected)
    {
        await using var connection = await Provider.OpenConnection();
        var result = await connection.ExecuteScalarAsync<string>("SELECT logix.tag_path(@tagName)", new { tagName });
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("BOOL", true)]
    [TestCase("SINT", true)]
    [TestCase("INT", true)]
    [TestCase("DINT", true)]
    [TestCase("LINT", true)]
    [TestCase("REAL", true)]
    [TestCase("LREAL", true)]
    [TestCase("USINT", true)]
    [TestCase("UINT", true)]
    [TestCase("UDINT", true)]
    [TestCase("ULINT", true)]
    [TestCase("DT", true)]
    [TestCase("LDT", true)]
    [TestCase("TIME32", true)]
    [TestCase("TIME", true)]
    [TestCase("LTIME", true)]
    [TestCase("STRING", false)]
    [TestCase("MY_STRUCT", false)]
    [TestCase("MyCustomType", false)]
    public async Task IsAtomicFunction_PostMigration_ShouldReturnExpectedValue(string dataType, bool expected)
    {
        await using var connection = await Provider.OpenConnection();
        var result = await connection.ExecuteScalarAsync<bool>("SELECT logix.is_atomic(@dataType)", new { dataType });
        Assert.That(result, Is.EqualTo(expected));
    }


    [TestCase("BOOL", "0")]
    [TestCase("SINT", "0")]
    [TestCase("INT", "0")]
    [TestCase("DINT", "0")]
    [TestCase("LINT", "0")]
    [TestCase("USINT", "0")]
    [TestCase("UINT", "0")]
    [TestCase("UDINT", "0")]
    [TestCase("ULINT", "0")]
    [TestCase("DT", "0")]
    [TestCase("LDT", "0")]
    [TestCase("TIME", "0")]
    [TestCase("TIME32", "0")]
    [TestCase("LTIME", "0")]
    [TestCase("REAL", "0.0")]
    [TestCase("LREAL", "0.0")]
    [TestCase("STRING", null)]
    public async Task DefaultValueFunction_PostMigration_ShouldReturnExpectedValue(string dataType, string? expected)
    {
        await using var connection = await Provider.OpenConnection();
        var result =
            await connection.ExecuteScalarAsync<string>("SELECT logix.default_value(@dataType)", new { dataType });
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task Hash_WithInput_ReturnsExpectedHash()
    {
        const string input = "test data";
        await using var connection = await Provider.OpenConnection();

        var result = await connection.ExecuteScalarAsync<string>("SELECT qa.hash(@input)", new { input });

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(64));
        Assert.That(result, Is.EqualTo("1F4A14C8F69FFA84088C6FF83F68E09271D2A63C92ECD0DC0781E84E7BA4DCD7"));
    }
}