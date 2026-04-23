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
    public async Task Purge_ConfirmedByUser_ShouldReturnZeroExitCode()
    {
        using var console = new FakeInMemoryConsole();
        console.WriteInput("y");
        var app = TestApp.Create(console, PurgeCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "purge",
            "-c", DbConnection,
            "-t", "Controller://Fake"
        ]);

        Assert.That(exitCode, Is.Zero);
    }
}
