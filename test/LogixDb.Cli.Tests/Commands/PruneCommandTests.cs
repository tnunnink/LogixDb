using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Cli.Common;

namespace LogixDb.Cli.Tests.Commands;

[TestFixture]
public class PruneCommandTests : TestDbFixture
{
    [SetUp]
    public Task Setup()
    {
        return Database.Migrate();
    }

    [Test]
    public async Task Prune_TargetNotFound_ShouldReturnZero()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, PruneCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "prune",
            "-c", DbConnection,
            "-t", "Controller://Fake"
        ]);

        Assert.That(exitCode, Is.Zero);
    }
}
