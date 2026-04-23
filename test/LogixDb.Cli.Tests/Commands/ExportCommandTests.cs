using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Cli.Common;
using LogixDb.Data;
using LogixDb.Testing;
using NUnit.Framework.Legacy;

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

    [Test]
    public async Task Export_ValidTarget_ShouldReturnZero()
    {
        //Generate and save L5X to the local directory for command.
        var testFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test.L5X");
        var source = TestSource.LocalTest();
        source.Save(testFile);
        await Database.PostTarget(Target.Create(source, "TestTarget"));

        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, ExportCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "export",
            "-c", DbConnection,
            "-t", "TestTarget",
            "-o", "output.L5X"
        ]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.Zero);
            Assert.That(File.Exists("output.L5X"), Is.True);
        }

        File.Delete("output.L5X");
    }
}