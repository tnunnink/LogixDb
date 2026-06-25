using System.Text;
using FluentAssertions;
using LogixDb.Testing;

namespace LogixDb.Data.Tests;

[TestFixture]
public class ImportTests
{
    [Test]
    public void Create_NoFileExtension_ShouldThrowException()
    {
        FluentActions.Invoking(() => _ = Import.Create("test", SourceType.API)).Should().Throw<ArgumentException>();
    }

    [Test]
    public void Create_EmptyFile_ShouldThrowException()
    {
        FluentActions.Invoking(() => _ = Import.Create("", SourceType.API)).Should().Throw<ArgumentException>();
    }

    [Test]
    public void Create_OnlyExtension_ShouldThrowException()
    {
        FluentActions.Invoking(() => _ = Import.Create(".L5X", SourceType.API)).Should().Throw<ArgumentException>();
    }

    [Test]
    public void Create_L5XFile_ReturnsCorrectImport()
    {
        using var import = Import.Create("test.L5X", SourceType.CLI);

        import.ImportId.Should().NotBeEmpty();
        import.Status.Should().Be(ImportStatus.Pending);
        import.SourceType.Should().Be(SourceType.CLI);
        import.FileType.Should().Be(FileType.L5X);
        import.FileName.Should().Be("test");
    }

    [Test]
    public void Create_ACDFile_ReturnsCorrectImport()
    {
        using var import = Import.Create("test.ACD", SourceType.API);

        import.ImportId.Should().NotBeEmpty();
        import.Status.Should().Be(ImportStatus.Pending);
        import.SourceType.Should().Be(SourceType.API);
        import.FileType.Should().Be(FileType.ACD);
    }

    [Test]
    [TestCase("test.l5x", "L5X")]
    [TestCase("test.acd", "ACD")]
    public void Create_LowerVariantExtension_ReturnsExpectedFileType(string fileName, string expected)
    {
        using var import = Import.Create(fileName, SourceType.CLI);

        import.FileType.Should().Be(Enum.Parse<FileType>(expected));
    }

    [Test]
    public void AddData_WithMetadata_PopulatesMetadata()
    {
        using var import = Import.Create("test.L5X", SourceType.FTAC);

        import.AddData(new Dictionary<string, string>
        {
            { "ProjectName", "TestProject" },
            { "Version", "1.0" }
        });

        import.Metadata.Should().HaveCount(2);
        import.Metadata["ProjectName"].Should().Be("TestProject");
        import.Metadata["Version"].Should().Be("1.0");
    }

    [Test]
    public void Info_ValidMessage_CreatesCorrectImportLog()
    {
        using var import = Import.Create("test.L5X", SourceType.CLI);

        var log = import.Info("Starting process");

        log.ImportId.Should().Be(import.ImportId);
        log.LogSeverity.Should().Be(LogSeverity.Info);
        log.LogMessage.Should().Be("Starting process");
        log.LogException.Should().BeNull();
    }

    [Test]
    public void Warning_ValidMessage_CreatesCorrectImportLog()
    {
        using var import = Import.Create("test.L5X", SourceType.CLI);

        var log = import.Warning("Potential issue detected");

        log.ImportId.Should().Be(import.ImportId);
        log.LogSeverity.Should().Be(LogSeverity.Warning);
        log.LogMessage.Should().Be("Potential issue detected");
        log.LogException.Should().BeNull();
    }

    [Test]
    public void Error_WithMessageAndException_CreatesCorrectImportLogWithException()
    {
        using var import = Import.Create("test.L5X", SourceType.CLI);
        var exception = new InvalidOperationException("Something went wrong");

        var log = import.Error("Critical failure", exception);

        log.ImportId.Should().Be(import.ImportId);
        log.LogSeverity.Should().Be(LogSeverity.Error);
        log.LogMessage.Should().Be("Critical failure");
        log.LogException.Should().NotBeNull();
        log.LogException.Should().Contain("InvalidOperationException");
        log.LogException.Should().Contain("Something went wrong");
    }

    [Test]
    public async Task WriteAsync_TestStream_FileShouldExist()
    {
        const string content = "test content";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var import = Import.Create("test.L5X", SourceType.CLI);

        var file = await import.WriteAsync(stream);

        file.Exists.Should().BeTrue();
        file.Length.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task WriteAsync_TestL5XFileOnDisc_FileShouldExistAndHaveExpectedSize()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "LogixDb");
        Directory.CreateDirectory(tempDir);
        var sourceFile = Path.Combine(tempDir, "Test.L5X");

        var content = TestSource.LocalTest();
        content.Save(sourceFile);

        using var import = Import.Create(sourceFile, SourceType.CLI);

        var file = await import.WriteAsync(sourceFile);

        file.Exists.Should().BeTrue();
        file.Length.Should().Be(new FileInfo(sourceFile).Length);
        Directory.Delete(tempDir, true);
    }

    [Test]
    public async Task OpenWriter_WhenCalled_ShouldNotBeNull()
    {
        using var import = Import.Create("test.L5X", SourceType.CLI);

        await using var writer = import.OpenWrite();

        writer.Should().NotBeNull();
    }

    [Test]
    public async Task OpenWriter_WhenWritten_ShouldUpdateExpectedFileContent()
    {
        var testData = "Hello"u8.ToArray();
        using var import = Import.Create("test.L5X", SourceType.CLI);

        var expectedFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "LogixDb",
            $"{import.FileName}.{import.ImportId}.{import.FileType}"
        );

        await using var writer = import.OpenWrite();
        await writer.WriteAsync(testData);
        await writer.FlushAsync();

        var file = new FileInfo(expectedFile);
        file.Exists.Should().BeTrue();
        file.Length.Should().Be(testData.Length);
    }

    [Test]
    public async Task Dispose_TestFile_FileShouldBeDeleted()
    {
        const string content = "test content";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var import = Import.Create("test.L5X", SourceType.CLI);

        var file = await import.WriteAsync(stream);

        file.Exists.Should().BeTrue();
        import.Dispose();
        file.Refresh();
        file.Exists.Should().BeFalse();
    }
}