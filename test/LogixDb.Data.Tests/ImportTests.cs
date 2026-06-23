using FluentAssertions;

namespace LogixDb.Data.Tests;

[TestFixture]
public class ImportTests
{
    [Test]
    public void Create_L5XFile_ReturnsCorrectImport()
    {
        var import = Import.Create("test.L5X", SourceType.CLI);

        import.ImportId.Should().NotBeEmpty();
        import.Status.Should().Be(ImportStatus.Pending);
        import.SourceType.Should().Be(SourceType.CLI);
        import.FileType.Should().Be(FileType.L5X);
        import.FileName.Should().Be("test");
        import.FilePath.Should().NotBeEmpty();
        import.SourceFile.Should().NotBeEmpty();
        import.GetTempFile().Should().Contain(import.ImportId.ToString("N"));
    }

    [Test]
    public void Create_ACDFile_ReturnsCorrectImport()
    {
        const string fileName = "test.ACD";
        const SourceType sourceType = SourceType.API;

        var import = Import.Create(fileName, sourceType);

        import.FileType.Should().Be(FileType.ACD);
        import.SourceType.Should().Be(sourceType);
        import.SourceFile.Should().EndWith(".ACD");
    }

    [Test]
    public void Create_ExplicitPath_ReturnsCorrectImport()
    {
        var tempDir = Path.GetTempPath();
        var sourceFile = Path.Combine(tempDir, "test.L5X");

        var import = Import.Create(sourceFile, SourceType.CLI);

        import.FileType.Should().Be(FileType.L5X);
        import.FileName.Should().Be("test");
        import.FilePath.Should().Be(Path.GetDirectoryName(Path.GetFullPath(sourceFile)));
    }

    [Test]
    public void Create_LowerVariantExtension_ReturnsExpectedFileType()
    {
        var import = Import.Create("test.l5x", SourceType.CLI);

        import.FileType.Should().Be(FileType.L5X);
    }

    [Test]
    public void AddData_WithMetadata_PopulatesMetadata()
    {
        var import = Import.Create("test.L5X", SourceType.FTAC);

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
    public void SourceFile_Property_FollowsExpectedFormat()
    {
        var tempDir = Path.GetTempPath();
        var sourceFile = Path.Combine(tempDir, "test.L5X");
        var import = Import.Create(sourceFile, SourceType.CLI);

        var fullPath = import.SourceFile;
        
        fullPath.Should().Be(Path.Combine(tempDir, "test.L5X"));
    }

    [Test]
    public void TempFile_Property_FollowsExpectedFormat()
    {
        var sourceFile = Path.Combine("C:\\Temp", "test.L5X");
        var import = Import.Create(sourceFile, SourceType.CLI);

        var fullPath = import.GetTempFile();

        fullPath.Should().Be(Path.Combine(
            Path.GetTempPath(),
            "LogixDb",
            $"test.{import.ImportId:N}.L5X")
        );
    }

    [Test]
    public void Info_ValidMessage_CreatesCorrectImportLog()
    {
        const string message = "Starting process";
        var import = Import.Create("test.L5X", SourceType.CLI);

        var log = import.Info(message);

        log.ImportId.Should().Be(import.ImportId);
        log.LogSeverity.Should().Be(LogSeverity.Info);
        log.LogMessage.Should().Be(message);
        log.LogException.Should().BeNull();
    }

    [Test]
    public void Warning_ValidMessage_CreatesCorrectImportLog()
    {
        const string message = "Potential issue detected";
        var import = Import.Create("test.L5X", SourceType.CLI);

        var log = import.NewWarning(message);

        log.ImportId.Should().Be(import.ImportId);
        log.LogSeverity.Should().Be(LogSeverity.Warning);
        log.LogMessage.Should().Be(message);
        log.LogException.Should().BeNull();
    }

    [Test]
    public void Error_WithMessageAndException_CreatesCorrectImportLogWithException()
    {
        const string message = "Critical failure";
        var import = Import.Create("test.L5X", SourceType.CLI);
        var exception = new InvalidOperationException("Something went wrong");

        var log = import.Error(message, exception);

        log.ImportId.Should().Be(import.ImportId);
        log.LogSeverity.Should().Be(LogSeverity.Error);
        log.LogMessage.Should().Be(message);
        log.LogException.Should().NotBeNull();
        log.LogException.Should().Contain("InvalidOperationException");
        log.LogException.Should().Contain("Something went wrong");
    }
}