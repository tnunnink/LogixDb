using Dapper;

namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260708Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM20260708001_CreatesTagBaseFunction()
    {
        await AssertFunctionExists("logix", "tag_base");
    }

    [TestCase("Tag", "Tag")]
    [TestCase("Tag.Member", "Tag")]
    [TestCase("Tag[0]", "Tag")]
    [TestCase("Tag.Member[0]", "Tag")]
    [TestCase("Tag[0].Member", "Tag")]
    public async Task TagBase_WithInput_ReturnsExpectedBase(string tagName, string expected)
    {
        await using var connection = await Provider.OpenConnection();
        var result = await connection.ExecuteScalarAsync<string>("SELECT logix.tag_base(@tagName)", new { tagName });
        Assert.That(result, Is.EqualTo(expected));
    }
}
