using CliFx.Infrastructure;
using LogixDb.Cli.Commands;
using LogixDb.Cli.Common;
using LogixDb.Data;
using LogixDb.Testing;

namespace LogixDb.Cli.Tests.Commands;

[TestFixture]
public class PruneCommandTests : TestDbFixture
{
    [SetUp]
    public Task Setup()
    {
        return Migrator.Migrate(Connection);
    }

    [Test]
    public async Task Prune_TargetNotFound_ShouldReturnZero()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, PruneCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "prune",
            "-c", Connection.Source,
            "-t", "Fake",
            "-v", "1"
        ]);

        Assert.That(exitCode, Is.Zero);
    }
    [Test]
    public async Task Prune_ValidTarget_ShouldReturnZero()
    {
        var target = Target.Create(TestSource.LocalTest(), "TestProject");
        await Manager.ImportTarget(target);

        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, PruneCommand.Descriptor);

        var exitCode = await app.RunAsync([
            "prune",
            "-c", Connection.Source,
            "-t", "TestProject",
            "-v", "1"
        ]);

        Assert.That(exitCode, Is.Zero);
        
        var results = (await Manager.ListTargets()).ToArray();
        Assert.That(results, Is.Empty);
    }
}
