namespace LogixDb.Data.Tests;

[TestFixture]
public class ImportTests
{
    private const string TestDropPath = "C:\\Temp";

    [Test]
    public void Create_L5XFile_ReturnsCorrectImport()
    {
        const string fileName = "test.L5X";
        const SourceType sourceType = SourceType.CLI;

        var import = Import.Create(fileName, TestDropPath, sourceType);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(import.ImportId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(import.ImportStatus, Is.EqualTo(ImportStatus.Pending));
            Assert.That(import.SourceType, Is.EqualTo(sourceType));
            Assert.That(import.FileType, Is.EqualTo(FileType.L5X));
            Assert.That(import.FileName, Is.EqualTo(fileName));
            Assert.That(import.DropPath, Is.EqualTo(TestDropPath));
            Assert.That(import.FilePath, Does.StartWith(TestDropPath));
            Assert.That(import.FilePath, Does.Contain(fileName));
            Assert.That(import.FilePath, Does.EndWith(".L5X"));
        }
    }

    [Test]
    public void Create_ACDFile_ReturnsCorrectImport()
    {
        const string fileName = "test.ACD";
        const SourceType sourceType = SourceType.API;

        var import = Import.Create(fileName, TestDropPath, sourceType);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(import.FileType, Is.EqualTo(FileType.ACD));
            Assert.That(import.SourceType, Is.EqualTo(sourceType));
            Assert.That(import.FilePath, Does.EndWith(".ACD"));
        }
    }

    [Test]
    public void Create_WithMetadata_PopulatesMetadata()
    {
        var metadata = new Dictionary<string, string>
        {
            { "ProjectName", "TestProject" },
            { "Version", "1.0" }
        };

        var import = Import.Create("test.L5X", TestDropPath, SourceType.FTAC, metadata);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(import.Metadata, Has.Count.EqualTo(2));
            Assert.That(import.Metadata["ProjectName"], Is.EqualTo("TestProject"));
            Assert.That(import.Metadata["Version"], Is.EqualTo("1.0"));
        }
    }

    [Test]
    public void FilePath_Property_FollowsExpectedFormat()
    {
        var import = Import.Create("Project.L5X", TestDropPath, SourceType.CLI);
        var expectedFileNamePart = $"Project.L5X.{import.ImportId:N}.L5X";

        var filePath = import.FilePath;

        Assert.That(filePath, Is.EqualTo(Path.Combine(TestDropPath, expectedFileNamePart)));
    }

    [Test]
    public void Info_ValidMessage_CreatesCorrectImportLog()
    {
        var import = Import.Create("test.L5X", TestDropPath, SourceType.CLI);
        const string message = "Starting process";

        var log = import.Info(message);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(log.ImportId, Is.EqualTo(import.ImportId));
            Assert.That(log.LogSeverity, Is.EqualTo(LogSeverity.Info));
            Assert.That(log.LogMessage, Is.EqualTo(message));
            Assert.That(log.LogException, Is.Null);
        }
    }

    [Test]
    public void NewWarning_ValidMessage_CreatesCorrectImportLog()
    {
        var import = Import.Create("test.L5X", TestDropPath, SourceType.CLI);
        const string message = "Potential issue detected";

        var log = import.NewWarning(message);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(log.ImportId, Is.EqualTo(import.ImportId));
            Assert.That(log.LogSeverity, Is.EqualTo(LogSeverity.Warning));
            Assert.That(log.LogMessage, Is.EqualTo(message));
            Assert.That(log.LogException, Is.Null);
        }
    }

    [Test]
    public void Error_WithMessageAndException_CreatesCorrectImportLogWithException()
    {
        var import = Import.Create("test.L5X", TestDropPath, SourceType.CLI);
        const string message = "Critical failure";
        var exception = new InvalidOperationException("Something went wrong");

        var log = import.Error(message, exception);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(log.ImportId, Is.EqualTo(import.ImportId));
            Assert.That(log.LogSeverity, Is.EqualTo(LogSeverity.Error));
            Assert.That(log.LogMessage, Is.EqualTo(message));
            Assert.That(log.LogException, Is.Not.Null);
            Assert.That(log.LogException, Does.Contain("InvalidOperationException"));
            Assert.That(log.LogException, Does.Contain("Something went wrong"));
        }
    }

    [Test]
    public void Create_LowerVariantExtension_ReturnsExpectedFileType()
    {
        var import = Import.Create("test.l5x", TestDropPath, SourceType.CLI);

        Assert.That(import.FileType, Is.EqualTo(FileType.L5X));
    }
}