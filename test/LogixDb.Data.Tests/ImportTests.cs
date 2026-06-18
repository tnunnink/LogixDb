namespace LogixDb.Data.Tests;

[TestFixture]
public class ImportTests
{
    [Test]
    public void Create_L5XFile_ShouldReturnCorrectSourceInfo()
    {
        var sourceInfo = Import.Create("test.L5X", "C:\\Temp", SourceType.CLI);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sourceInfo.ImportId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(sourceInfo.FileType, Is.EqualTo(FileType.L5X));
            Assert.That(sourceInfo.FileName, Is.EqualTo("test.L5X"));
            Assert.That(sourceInfo.DropPath, Does.StartWith("C:\\Temp"));
            Assert.That(sourceInfo.DropPath, Does.EndWith(".L5X"));
        }
    }

    [Test]
    public void Create_ACDFile_ShouldReturnCorrectSourceInfo()
    {
        var sourceInfo = Import.Create("test.ACD", "C:\\Temp", SourceType.CLI);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sourceInfo.FileType, Is.EqualTo(FileType.ACD));
            Assert.That(sourceInfo.DropPath, Does.EndWith(".ACD"));
        }
    }
}