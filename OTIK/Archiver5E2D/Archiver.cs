using System.Collections.ObjectModel;
using Archiver5E2D.Compressors;
using Archiver5E2D.Entities;
using Archiver5E2D.Interfaces;
using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D;

public static class Archiver
{
    public const string Extension = ".5e2d";

    private static readonly ReadOnlyCollection<Compressor> Compressors = new List<Compressor>
    {
        new CompressorV1()
    }.AsReadOnly();

    public static File Archive(string path)
    {
        if (System.IO.File.Exists(path))
            return ArchiveFile(path);
        if (Directory.Exists(path))
            return ArchiveFolder(path);
        throw new ArgumentException("The path does not lead to a file or folder");
    }
    
    public static IEnumerable<IEntity> Dearchive(string path)
    {
        if (System.IO.File.Exists(path))
        {
            // TODO: Добавить провайдер компрессоров для выбора нужной версии
            var compressors = GetLatestCompressor();
            var file = compressors.Decompress(File.FromExistingFile(path));
            return EntitiesConverter.Separate(file.Content);
        }
            
        throw new ArgumentException("The path does not lead to a file or folder");
    }

    private static File ArchiveFile(string path)
    {
        var compressors = GetLatestCompressor();
        var file = File.FromExistingFile(path);
        
        file = EntitiesConverter.Combine(new List<IEntity> { file }, file.Path,
            Path.GetFileNameWithoutExtension(path) + Extension);
        return compressors.Compress(file);
    }

    private static File ArchiveFolder(string path)
    {
        var compressors = GetLatestCompressor();
        var folder = Folder.FromExistingFolder(path);
        
        var file = EntitiesConverter.Combine(new List<IEntity> { folder }, folder.Path,
            folder.Name + Extension);
        return compressors.Compress(file);
    }

    private static Compressor GetLatestCompressor()
    {
        var latestVersion = Compressors.Max(compressor => compressor.Version);
        return Compressors.Single(compressor => compressor.Version == latestVersion);
    }
}