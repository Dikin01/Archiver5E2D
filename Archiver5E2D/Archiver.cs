using Archiver5E2D.Entities;
using Archiver5E2D.Interfaces;
using Archiver5E2D.Providers;
using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D;

public static class Archiver
{
    public const string Extension = ".5e2d";

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
            var archivedFile = File.FromExistingFile(path);
            var compressor = CompressorProvider.Provide(archivedFile);
            var dearchivedFile = compressor.Decompress(File.FromExistingFile(path));
            return EntitiesConverter.Separate(dearchivedFile.Content);
        }

        throw new ArgumentException("The path does not lead to a file");
    }

    private static File ArchiveFile(string path)
    {
        var compressor = CompressorProvider.GetLatestCompressor();
        var file = File.FromExistingFile(path);

        file = EntitiesConverter.Combine(new List<IEntity> { file }, file.Path,
            Path.GetFileNameWithoutExtension(path) + Extension);
        return compressor.Compress(file);
    }

    private static File ArchiveFolder(string path)
    {
        var compressors = CompressorProvider.GetLatestCompressor();
        var folder = Folder.FromExistingFolder(path);

        var file = EntitiesConverter.Combine(new List<IEntity> { folder }, folder.Path,
            folder.Name + Extension);
        return compressors.Compress(file);
    }
}