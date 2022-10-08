using Archiver5E2D.Compressors;
using Archiver5E2D.Entities;
using Archiver5E2D.Interfaces;
using Archiver5E2D.Providers;
using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D;

public static class Archiver
{
    private static readonly Compressor NewestCompressor = CompressorProvider.GetNewestCompressor();

    public const string Extension = ".5e2d";

    public static File Archive(string path)
    {
        IEntity entity;
        if (System.IO.File.Exists(path)) entity = File.FromExisting(path);
        else if (Directory.Exists(path)) entity = Folder.FromExisting(path);
        else throw new ArgumentException("The path does not lead to a file or directory");

        return Archive(entity);
    }

    public static IEnumerable<IEntity> Dearchive(string path)
    {
        if (!System.IO.File.Exists(path))
            throw new ArgumentException("The path does not lead to a file");

        var archivedFile = File.FromExisting(path);
        var compressor = CompressorProvider.ProvideForCompressedFile(archivedFile);
        var combinedFile = compressor.Decompress(archivedFile);

        return EntitiesConverter.SplitIntoEntities(combinedFile);
    }

    private static File Archive(IEntity entity)
    {
        var combinedFile = ConvertToCombinedFile(entity);
        return NewestCompressor.Compress(combinedFile);
    }

    private static File ConvertToCombinedFile(IEntity entity)
    {
        var name = Path.GetFileNameWithoutExtension(entity.Name) + Extension;
        return EntitiesConverter.CombineToFile(new List<IEntity> { entity }, entity.Path, name);
    }
}