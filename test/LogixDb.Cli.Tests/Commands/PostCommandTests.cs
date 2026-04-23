using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Cli.Common;

namespace LogixDb.Cli.Tests.Commands;

[TestFixture]
public class PostCommandTests : TestDbFixture
{
    [SetUp]
    public Task Setup()
    {
        return Database.Migrate();
    }

    [Test]
    public async Task Post_FileNotFound_ShouldReturnFileNotFound()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, PostCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "post",
            "-c", DbConnection,
            "-s", "nonexistent.l5x"
        ]);

        Assert.That(exitCode, Is.EqualTo(ErrorCodes.FileNotFound));
    }
}
