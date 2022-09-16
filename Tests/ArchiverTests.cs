using System.Text;
using Archiver5E2D;
using Archiver5E2D.Entities;
using Archiver5E2D.Interfaces;
using File = System.IO.File;

namespace Tests;

public class ArchiverTests
{
    [Fact]
    public void Archive_ShouldThrowArgumentException_WhenFileIsNotExist()
    {
        var path = Environment.CurrentDirectory;
        const string name = nameof(Archive_ShouldThrowArgumentException_WhenFileIsNotExist);
        const string extension = ".txt";
        var fullPath = Path.Combine(path, name + extension);
        File.Delete(fullPath);

        var act = () => Archiver.Archive(fullPath);

        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Dearchive_ShouldThrowArgumentException_WhenFileIsNotExist()
    {
        var path = Environment.CurrentDirectory;
        const string name = nameof(Dearchive_ShouldThrowArgumentException_WhenFileIsNotExist);
        const string extension = Archiver.Extension;
        var fullPath = Path.Combine(path, name + extension);
        File.Create(fullPath).Dispose();
        File.Delete(fullPath);

        var act = () => Archiver.Dearchive(fullPath);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ArchiveThenDearchive_ShouldReturnOriginFile_WhenFileIsExist()
    {
        var path = Environment.CurrentDirectory;
        const string name = nameof(ArchiveThenDearchive_ShouldReturnOriginFile_WhenFileIsExist);
        const string extension = ".txt";
        var fullPath = Path.Combine(path, name + extension);
        File.Create(fullPath).Dispose();
        File.WriteAllText(fullPath, "foo bar");
        var originFile = Archiver5E2D.Entities.File.FromExistingFile(fullPath);

        var archiveFile = Archiver.Archive(fullPath);
        archiveFile.Save(archiveFile.Path);
        var dearchiveEntities = Archiver.Dearchive(Path.Combine(archiveFile.Path, archiveFile.Name));
        var result = dearchiveEntities.First() as Archiver5E2D.Entities.File;
        File.Delete(fullPath);

        TestArchiveFile(archiveFile, path, name);
        result.Should().Be(originFile);
    }

    [Fact]
    public void ArchiveThenDearchive_ShouldReturnOriginFolder_WhenFolderContainFoldersAndFiles()
    {
        var rootPath = Path.Combine(Environment.CurrentDirectory,
            nameof(ArchiveThenDearchive_ShouldReturnOriginFile_WhenFileIsExist));
        var file1 = new Archiver5E2D.Entities.File("", "file1.txt", 
            Encoding.Default.GetBytes("foo bar"));
        var file2 = new Archiver5E2D.Entities.File("", "file2.mp3", Array.Empty<byte>());
        var folder1 = new Folder("", "folder1", new List<IEntity> { file1 });
        var folder2 = new Folder("", "folder2", new List<IEntity> { file1, file2 });
        var folder = new Folder(rootPath, "folder", new List<IEntity> { file1, file2, folder1, folder2});
        var folderFullPath = Path.Combine(folder.Path, folder.Name);
        folder.Save(rootPath);
        var resultPath = Path.Combine(rootPath, "result");
        Directory.CreateDirectory(resultPath);
        
        var archiveFile = Archiver.Archive(folderFullPath);
        archiveFile.Save(resultPath);
        var dearchiveEntities = Archiver.Dearchive(Path.Combine(resultPath, archiveFile.Name));
        
        foreach (var entity in dearchiveEntities)
        {
            entity.Save(resultPath);
        }
        Directory.Delete(rootPath, true);
        TestArchiveFile(archiveFile, rootPath, folder.Name);
        var resultFolder = dearchiveEntities.First() as Folder;
        resultFolder.Entities.Should().BeEquivalentTo(folder.Entities,
            options => options.Using(new TestsIEntityComparator()));
    }

    private static void TestArchiveFile(Archiver5E2D.Entities.File file, string expectedRootPath,
        string expectedFileName)
    {
        file.Path.Should().Be(expectedRootPath);
        file.Name.Should().Be(expectedFileName + Archiver.Extension);
        file.Content.Should().NotBeEmpty();
    }

    private class TestsIEntityComparator : IEqualityComparer<IEntity>
    {
        public bool Equals(IEntity? x, IEntity? y)
        {
            if (x.GetType() != y.GetType())
                return false;

            if (x.GetType() == typeof(Archiver5E2D.Entities.File))
                return x.Name == y.Name && x.Content.SequenceEqual(y.Content);
            
            if (x.GetType() == typeof(Folder))
                return x.Name == y.Name && 
                       (x as Folder).Entities.All(ent => (y as Folder).Entities.Contains(ent, this));
            
            return false;
        }

        public int GetHashCode(IEntity obj)
        {
            return HashCode.Combine(obj.Name, obj.Content, obj.TypeId, obj.Path);
        }
    }
}