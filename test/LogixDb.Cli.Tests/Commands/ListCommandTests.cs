using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Data;
using LogixDb.Testing;

namespace LogixDb.Cli.Tests.Commands;

[TestFixture]
public class ListCommandTests : TestDbFixture
{
    [SetUp]
    public Task Setup()
    {
        return Database.Migrate();
    }

    [Test]
    public async Task List_EmptyDatabase_ShouldReturnZeroExitCode()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, ListCommand.Descriptor);

        var exitCode = await app.RunAsync(["list", "-c", DbConnection]);
        Assert.That(exitCode, Is.Zero);
    }

    [Test]
    public async Task List_WithTargetFilter_ShouldReturnZeroExitCode()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, ListCommand.Descriptor);

        var exitCode = await app.RunAsync(
        [
            "list",
            "-c", DbConnection,
            "-t", "controller://TestController"
        ]);

        Assert.That(exitCode, Is.Zero);
    }

    [Test]
    public async Task List_WithSingleTarget_ShouldReturnZeroExitCode()
    {
        var target = Target.Create(TestSource.LocalTest());
        await Database.ImportTarget(target);

        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, ListCommand.Descriptor);

        var exitCode = await app.RunAsync(["list", "-c", DbConnection]);

        Assert.That(exitCode, Is.Zero);
    }
}
