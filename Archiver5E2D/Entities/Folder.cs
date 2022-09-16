using Archiver5E2D.Interfaces;
using SystemFile = System.IO.File;

namespace Archiver5E2D.Entities;

public class Folder : IEntity
{
    public List<IEntity> Entities { get; } = new();
    public string Path { get; }
    public string Name { get; }
    public byte[] Content => EntitiesConverter.CombineToFile(Entities, Path, Name).Content;
    public byte TypeId => GetTypeId();
    public static byte GetTypeId() => 1;

    public Folder(string path, string name)
    {
        Path = path;
        Name = name;
    }

    public Folder(string path, string name, List<IEntity> entities)
        : this(path, name)
    {
        Entities.AddRange(entities);
    }

    public static Folder FromExistingFolder(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Exists)
            throw new ArgumentException("The path does not lead to the file.");

        var name = directoryInfo.Name;
        var directoryPath = directoryInfo.Parent?.FullName!;
        var folder = new Folder(directoryPath, name);

        var entities = folder.Entities;
        
        entities.AddRange(directoryInfo
            .GetFiles()
            .Select(fileInfos => File.FromExistingFile(fileInfos.FullName)));

        entities.AddRange(directoryInfo
            .GetDirectories()
            .Select(fileInfos => FromExistingFolder(fileInfos.FullName)));

        return folder;
    }

    public void Save(string rootPath)
    {
        var fullPath = System.IO.Path.Combine(rootPath, Name);
        Directory.CreateDirectory(fullPath);
        foreach (var entity in Entities)
            entity.Save(fullPath);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Folder folder)
            return Equals(folder);
        return false;
    }

    private bool Equals(Folder other)
    {
        return Path == other.Path &&
               Name == other.Name &&
               Entities.Count == other.Entities.Count &&
               Entities.All(entity => other.Entities.Contains(entity));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Path, Name, Content, TypeId);
    }
}