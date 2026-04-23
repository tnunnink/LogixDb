using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Cli.Common;

namespace LogixDb.Cli.Tests.Commands;

[TestFixture]
public class SyncCommandTests : TestDbFixture
{
    [SetUp]
    public Task Setup()
    {
        return Database.Migrate();
    }

    [Test]
    public async Task Sync_TargetNotFound_ShouldReturnFileNotFound()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, SyncCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "sync",
            "-c", DbConnection,
            "-t", "Controller://Fake"
        ]);

        Assert.That(exitCode, Is.EqualTo(ErrorCodes.FileNotFound));
    }
}
