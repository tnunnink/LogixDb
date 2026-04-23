using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Cli.Common;

namespace LogixDb.Cli.Tests.Commands;

[TestFixture]
public class PurgeCommandTests : TestDbFixture
{
    [SetUp]
    public Task Setup()
    {
        return Database.Migrate();
    }

    [Test]
    public async Task Purge_WithForce_ShouldReturnZero()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, PurgeCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "purge",
            "-c", DbConnection,
            "-t", "Controller://Test",
            "--force"
        ]);

        Assert.That(exitCode, Is.Zero);
    }
}
