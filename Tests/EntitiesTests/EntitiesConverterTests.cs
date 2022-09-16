using Archiver5E2D.Entities;
using Archiver5E2D.Interfaces;
using File = Archiver5E2D.Entities.File;

namespace Tests.EntitiesTests;

public class EntitiesConverterTests
{
    [Fact]
    public void CombineThenSeparate_ShouldReturnOriginalFiles_WhenCombine2Files()
    {
        const string newPath = "newPath";
        const string newName = "newName";
        var longPath = new string('a', 265);

        var file1 = new File(longPath, "file1.txt", new byte[] { 0xAA });
        var file2 = new File("path2", "file2.docx", new byte[] { 0xBB });
        var files = new List<IEntity> { file1, file2 };

        var combinedFile = EntitiesConverter.CombineToFile(files, newPath, newName);
        var result = EntitiesConverter.SplitIntoEntities(combinedFile);
        result.Should().Equal(files);
    }

    [Fact]
    public void CombineThenSeparate_ShouldReturnOriginalFiles_WhenCombineFolderContain2Files()
    {
        const string newPath = "newPath";
        const string newName = "newName";
        var longPath = new string('a', 265);

        var file1 = new File(longPath, "file1.txt", new byte[] { 0xAA });
        var file2 = new File("path2", "file2.docx", new byte[] { 0xBB });
        var files = new List<IEntity> { file1, file2 };
        var folder = new Folder("path3", "folder", files);
        var packedFolder = new List<IEntity> { folder };

        var combinedFile = EntitiesConverter.CombineToFile(packedFolder, newPath, newName);
        var result = EntitiesConverter.SplitIntoEntities(combinedFile);
        result.Should().Equal(packedFolder);
    }

    [Fact]
    public void CombineThenSeparate_ShouldReturnOriginalFiles_WhenCombineFolderContainFoldersAndFiles()
    {
        const string newPath = "newPath";
        const string newName = "newName";
        var longPath = new string('a', 265);
        var file1 = new File(longPath, "file1.txt", new byte[] { 0xAA });
        var file2 = new File("path2", "file2.docx", new byte[] { 0xBB });
        var files = new List<IEntity> { file1, file2 };
        var folder1 = new Folder("path3", "folder");
        var folder2 = new Folder("path4", "folder");
        folder2.Entities.Add(folder1);
        folder2.Entities.AddRange(files);
        var packedFolder = new List<IEntity> { folder2 };

        var combinedFile = EntitiesConverter.CombineToFile(packedFolder, newPath, newName);
        var result = EntitiesConverter.SplitIntoEntities(combinedFile);
        result.Should().Equal(packedFolder);
    }
}