using CliFx.Infrastructure;
using LogixDb.Cli.Commands;

namespace LogixDb.Cli.Tests.Commands;

[TestFixture]
public class DropCommandTests : TestDbFixture
{
    [SetUp]
    public Task Setup()
    {
        return Database.Migrate();
    }
    
    [Test]
    public async Task Drop_ConfirmedByUser_ShouldReturnZeroExitCode()
    {
        using var console = new FakeInMemoryConsole();
        var app = TestApp.Create(console, DropCommand.Descriptor);

        console.WriteInput("y");
        
        var exitCode = await app.RunAsync([
            "drop",
            "-c", DbConnection
        ]);
        
        Assert.That(exitCode, Is.Zero);
    }
}