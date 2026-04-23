using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Cli.Common;
using LogixDb.Data;
using LogixDb.Testing;

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

    [Test]
    public async Task Post_ValidFile_ShouldReturnZero()
    {
        //Generate and save L5X to the local directory for command.
        var testFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test.L5X");
        var source = TestSource.LocalTest();
        source.Save(testFile);
        await Database.PostTarget(Target.Create(source, "TestTarget"));

        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, PostCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "post",
            "-c", DbConnection,
            "-s", testFile
        ]);

        Assert.That(exitCode, Is.Zero);
    }
}