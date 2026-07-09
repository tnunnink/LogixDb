using Dapper;

namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260709Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM20260709001_CreatesTagMemberFunction()
    {
        await AssertFunctionExists("logix", "tag_path");
    }

    [TestCase("Tag", "")]
    [TestCase("Tag.Member", "Member")]
    [TestCase("Tag[0]", "[0]")]
    [TestCase("Tag.Member[0]", "Member[0]")]
    [TestCase("Tag[0].Member", "[0].Member")]
    public async Task TagMember_WithInput_ReturnsExpectedMember(string tagName, string expected)
    {
        await using var connection = await Provider.OpenConnection();
        var result = await connection.ExecuteScalarAsync<string>("SELECT logix.tag_path(@tagName)", new { tagName });
        Assert.That(result, Is.EqualTo(expected));
    }
}
