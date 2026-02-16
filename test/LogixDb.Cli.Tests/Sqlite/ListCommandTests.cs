using LogixDb.Cli.Commands;

namespace LogixDb.Cli.Tests.Sqlite;

[TestFixture]
public class ListCommandTests
{
    [Test]
    public async Task List_EmptyDatabase_ShouldReturnZeroExitCode()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        var app = TestApp.Create<ListCommand>(out var console);

        // Migrate database first
        var migrateApp = TestApp.Create<MigrateCommand>(out var migrateConsole);
        await migrateApp.RunAsync(["migrate", "-c", tempPath]);
        migrateConsole.Dispose();

        // Run list command
        var exitCode = await app.RunAsync(["list", "-c", tempPath]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.Zero);
            var output = console.ReadOutputString();
            Assert.That(output, Does.Contain("No snapshots found"));
        }

        console.Dispose();

        if (File.Exists(tempPath)) File.Delete(tempPath);
    }

    [Test]
    public async Task List_WithTargetFilter_ShouldReturnZeroExitCode()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        var app = TestApp.Create<ListCommand>(out var console);

        // Migrate database first
        var migrateApp = TestApp.Create<MigrateCommand>(out var migrateConsole);
        await migrateApp.RunAsync(["migrate", "-c", tempPath]);
        migrateConsole.Dispose();

        // Run list command with target filter
        var exitCode = await app.RunAsync(["list", "-c", tempPath, "-t", "controller://TestController"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.Zero);
        }

        console.Dispose();

        if (File.Exists(tempPath)) File.Delete(tempPath);
    }
}