using Dapper;

namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260709Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_M20260709_CreatesFunctions()
    {
        await AssertFunctionExists("logix", "tag_path");
        await AssertFunctionExists("logix", "is_atomic");
        await AssertFunctionExists("logix", "default_value");
    }

    [TestCase("Tag", "")]
    [TestCase("Tag.Member", "Member")]
    [TestCase("Tag[0]", "[0]")]
    [TestCase("Tag.Member[0]", "Member[0]")]
    [TestCase("Tag[0].Member", "[0].Member")]
    public async Task TagPath_WithInput_ReturnsExpectedPath(string tagName, string expected)
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
    public async Task IsAtomic_WithInput_ReturnsExpected(string dataType, bool expected)
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
    public async Task DefaultValue_WithInput_ReturnsExpectedValue(string dataType, string? expected)
    {
        await using var connection = await Provider.OpenConnection();
        var result = await connection.ExecuteScalarAsync<string>("SELECT logix.default_value(@dataType)", new { dataType });
        Assert.That(result, Is.EqualTo(expected));
    }
}
