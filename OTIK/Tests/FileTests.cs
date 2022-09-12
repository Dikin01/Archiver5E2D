using File = Archiver5E2D.Entities.File;

namespace Tests;

public class FileTests
{
    [Fact]
    public void CombineThanSeparate_ShouldReturnOriginalFiles()
    {
        const string newPath = "newPath";
        const string newName = "newName";
        var longPath = new string('a', 265);
        
        var file1 = new File(longPath,"file1.txt", new byte[] { 0xAA });
        var file2 = new File("path2", "file2.docx", new byte[] { 0xBB });
        var files = new List<File> { file1, file2 };

        var combinedFile = File.Combine(files, newPath, newName);
        var result = File.Separate(combinedFile.Content);
        result.Should().Equal(files);
    }
}