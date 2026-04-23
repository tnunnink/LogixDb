using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Cli.Common;

namespace LogixDb.Cli.Tests.Commands;

[TestFixture]
public class ExportCommandTests : TestDbFixture
{
    [SetUp]
    public Task Setup()
    {
        return Database.Migrate();
    }

    [Test]
    public async Task Export_TargetNotFound_ShouldReturnTargetNotFound()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, ExportCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "export",
            "-c", DbConnection,
            "-t", "Controller://Fake",
            "-o", "output.l5x"
        ]);

        Assert.That(exitCode, Is.EqualTo(ErrorCodes.TargetNotFound));
    }
}
