using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Sqlite;
using LogixDb.Data.SqlServer;
using LogixDb.Service.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogixDb.Service.Tests.Common;

[TestFixture]
public class AddLogixDbTests
{
    private IServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
        _services.AddLogging(); // This registers ILoggerFactory and ILogger<>
    }

    [Test]
    public void AddLogixDb_SqliteProvider_ShouldRegisterSqliteManager()
    {
        var config = new LogixConfig
        {
            DbConnection = "test.db"
        };

        _services.AddLogixDb(config);
        var provider = _services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IDbManager>();

        Assert.That(manager, Is.InstanceOf<SqliteManager>());
    }

    [Test]
    public void AddLogixDb_SqlServerProvider_ShouldRegisterSqlServerManager()
    {
        var config = new LogixConfig
        {
            DbConnection = "Database@Server"
        };

        _services.AddLogixDb(config);
        var provider = _services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IDbManager>();

        Assert.That(manager, Is.InstanceOf<SqlServerManager>());
    }

    [Test]
    public void AddLogixDb_WhenResolved_ShouldNotThrowInvalidOperationException()
    {
        var config = new LogixConfig
        {
            DbConnection = "test.db"
        };

        _services.AddLogixDb(config);
        var provider = _services.BuildServiceProvider();

        // This is where it would have failed before
        Assert.DoesNotThrow(() => provider.GetRequiredService<IDbManager>());
    }
}
