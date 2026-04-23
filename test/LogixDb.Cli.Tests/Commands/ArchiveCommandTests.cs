using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Cli.Common;

namespace LogixDb.Cli.Tests.Commands;

[TestFixture]
public class ArchiveCommandTests : TestDbFixture
{
    [SetUp]
    public Task Setup()
    {
        return Database.Migrate();
    }

    [Test]
    public async Task Archive_TargetNotFound_ShouldReturnInternalError()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, ArchiveCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "archive",
            "-c", DbConnection,
            "-t", "Controller://Fake"
        ]);

        Assert.That(exitCode,  Is.EqualTo(ErrorCodes.InternalError));
    }
}