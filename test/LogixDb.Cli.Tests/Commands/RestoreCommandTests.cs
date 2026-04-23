using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Cli.Common;
using LogixDb.Data;
using LogixDb.Testing;

namespace LogixDb.Cli.Tests.Commands;

[TestFixture]
public class RestoreCommandTests : TestDbFixture
{
    [SetUp]
    public Task Setup()
    {
        return Database.Migrate();
    }

    [Test]
    public async Task Restore_TargetNotFound_ShouldReturnInternalError()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, RestoreCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "restore",
            "-c", DbConnection,
            "-t", "Controller://Fake"
        ]);

        Assert.That(exitCode, Is.EqualTo(ErrorCodes.InternalError));
    }

    [Test]
    public async Task Restore_ValidTarget_ShouldReturnZero()
    {
        //Generate and save L5X to the local directory for command.
        var testFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test.L5X");
        var source = TestSource.LocalTest();
        source.Save(testFile);
        await Database.PostTarget(Target.Create(source, "TestTarget"));

        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, RestoreCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "restore",
            "-c", DbConnection,
            "-t", "TestTarget"
        ]);

        Assert.That(exitCode, Is.Zero);
    }
}