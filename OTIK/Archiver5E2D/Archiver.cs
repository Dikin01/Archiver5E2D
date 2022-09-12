using System.Collections.ObjectModel;
using Archiver5E2D.Compressors;
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

    private static Compressor GetLatestCompressor()
    {
        var latestVersion = Compressors.Max(compressor => compressor.Version);
        return Compressors.Single(compressor => compressor.Version == latestVersion);
    }

    private static File ArchiveFile(string path)
    {
        var compressors = GetLatestCompressor();
        
        var bytes = System.IO.File.ReadAllBytes(path);
        var directoryName = Path.GetDirectoryName(path)!;
        var fileName = Path.GetFileNameWithoutExtension(path);
        
        var file = new File(directoryName, fileName + Extension, bytes);
        return compressors.Compress(file);
    }

    private static File ArchiveFolder(string path)
    {
        throw new NotImplementedException();
    }
}