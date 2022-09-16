using System.Collections.ObjectModel;
using Archiver5E2D.Compressors;
using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D.Providers;

public static class CompressorProvider
{
    private static readonly ReadOnlyCollection<Compressor> Compressors;

    static CompressorProvider()
    {
        var typeCompressor = typeof(Compressor);
        var compressorHeirsTypes = typeCompressor.Assembly
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeCompressor) && type.IsClass && !type.IsAbstract);
        Compressors = compressorHeirsTypes
            .Select(type => (Compressor)Activator.CreateInstance(type)!)
            .ToList().AsReadOnly();
    }

    public static Compressor ProvideForCompressedFile(File file)
    {
        var version = file.Content[(int)Compressor.VersionOffset];
        return GetCompressorByVersion(version);
    }

    public static Compressor GetNewestCompressor()
    {
        var newVersion = Compressors.Max(compressor => compressor.Version);
        return GetCompressorByVersion(newVersion);
    }

    private static Compressor GetCompressorByVersion(byte version)
    {
        return Compressors.Single(compressor => compressor.Version == version);
    }
}