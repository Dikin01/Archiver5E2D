using Archiver5E2D;

namespace Tests;

public class ArchiverTests
{
    [Fact]
    public void Archive_ShouldReturnCompressedFile_WhenInputFileIsExist()
    {
        var path = Environment.CurrentDirectory;
        const string name = nameof(Archive_ShouldReturnCompressedFile_WhenInputFileIsExist);
        const string extension = ".txt";
        var fullPath = Path.Combine(path, name + extension);
        File.Create(fullPath).Dispose();

        var file = Archiver.Archive(fullPath);

        file.Path.Should().Be(path);
        file.Name.Should().ContainAll(new List<string> { name, Archiver.Extension });
        file.Name.Should().Contain(Archiver.Extension);
        file.Content.Should().NotBeEmpty();
        
        File.Delete(fullPath);
    }
    
    [Fact]
    public void Archive_ShouldThrowArgumentException_WhenInputFileIsNotExist()
    {
        var path = Environment.CurrentDirectory;
        const string name = nameof(Archive_ShouldThrowArgumentException_WhenInputFileIsNotExist);
        const string extension = ".txt";
        var fullPath = Path.Combine(path, name + extension);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        var act = () => Archiver.Archive(fullPath);

        act.Should().Throw<ArgumentException>();
    }
}