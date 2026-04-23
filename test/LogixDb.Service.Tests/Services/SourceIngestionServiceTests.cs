using System.Threading.Channels;
using L5Sharp.Core;
using LogixConverter.Abstractions;
using LogixDb.Data;
using LogixDb.Data.Sqlite;
using LogixDb.Service.Common;
using LogixDb.Service.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;
using Moq;

namespace LogixDb.Service.Tests.Services;

[TestFixture]
public class SourceIngestionServiceTests
{
    private Mock<ILogixFileConverter> _fileConverterMock;
    private Mock<ILogger<SourceIngestionService>> _loggerMock;
    private Mock<IOptions<LogixConfig>> _optionsMock;
    private Channel<SourceInfo> _channel;
    private string _testDbPath;
    private string _testDropPath;
    private SqliteManager _dbManager;

    [SetUp]
    public void Setup()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"LogixDb_{Guid.NewGuid():N}.db");
        _testDropPath = Path.Combine(Path.GetTempPath(), "LogixDbUploads", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testDropPath);

        var connectionInfo = DbConnectionInfo.Parse(_testDbPath);
        _dbManager = new SqliteManager(connectionInfo, new FakeLogger());
        // We need to migrate the database for it to be valid
        _dbManager.Migrate().GetAwaiter().GetResult();

        _fileConverterMock = new Mock<ILogixFileConverter>();
        _loggerMock = new Mock<ILogger<SourceIngestionService>>();
        _optionsMock = new Mock<IOptions<LogixConfig>>();
        _optionsMock.Setup(o => o.Value).Returns(new LogixConfig
        {
            DbConnection = _testDbPath,
            DropPath = _testDropPath
        });

        _channel = Channel.CreateUnbounded<SourceInfo>();
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_testDbPath)) File.Delete(_testDbPath);
        if (Directory.Exists(_testDropPath)) Directory.Delete(_testDropPath, true);
    }

    [Test]
    public async System.Threading.Tasks.Task ExecuteAsync_ValidSource_ShouldProcessAndAddToDb()
    {
        // Arrange
        var sourceId = Guid.NewGuid();
        const string fileName = "test.L5X";
        var filePath = Path.Combine(_testDropPath, $"{sourceId:N}.L5X");

        // Create a dummy L5X file
        var l5X = L5X.Parse(
            "<RSLogix5000Content TargetName=\"TestController\" TargetType=\"Controller\" SchemaRevision=\"1.0\"></RSLogix5000Content>");
        l5X.Save(filePath);

        var source = new SourceInfo
        {
            SourceId = sourceId,
            FileType = FileType.L5X,
            FileName = fileName,
            FilePath = filePath
        };

        var service = new SourceIngestionService(
            _channel,
            _dbManager,
            _fileConverterMock.Object,
            _optionsMock.Object,
            _loggerMock.Object);

        using var cts = new CancellationTokenSource();
        var startTask = service.StartAsync(cts.Token);

        // Act
        await _channel.Writer.WriteAsync(source, cts.Token);

        // Wait a bit for processing
        await System.Threading.Tasks.Task.Delay(1000, cts.Token);

        // Assert
        var targets = (await _dbManager.ListTargets(token: cts.Token)).ToList();
        Assert.That(targets, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(targets.First().TargetName, Is.EqualTo("TestController"));
            Assert.That(File.Exists(filePath), Is.False, "Original file should be deleted");
        }

        await service.StopAsync(cts.Token);
        await startTask;
    }
}