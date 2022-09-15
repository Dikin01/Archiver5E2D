using System.Collections.ObjectModel;
using System.Reflection;
using Archiver5E2D.Compressors;
using Archiver5E2D.Exceptions;
using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D.Providers;

public static class CompressorProvider
{
    private static readonly ReadOnlyCollection<Compressor> Compressors = new List<Compressor>
    {
        new CompressorV1()
    }.AsReadOnly();

    static CompressorProvider()
    {
        var compressors = Assembly.GetExecutingAssembly().GetExportedTypes()
            .Where(type => type.IsSubclassOf(typeof(Compressor)));
    }

    public static Compressor Provide(File compressedFile)
    {
        var version = compressedFile.Content[(int)Compressor.VersionOffset];
        return GetCompressorByVersion(version);
    }

    public static Compressor GetLatestCompressor()
    {
        var latestVersion = Compressors.Max(compressor => compressor.Version);
        return GetCompressorByVersion(latestVersion);
    }

    private static Compressor GetCompressorByVersion(byte version)
    {
        try
        {
            return Compressors.Single(compressor => compressor.Version == version);
        }
        catch (InvalidOperationException e)
        {
            throw new CompressorNotFoundException("There isn't one-to-one correspondence" +
                                                  " between version and compressor", e);
        }
    }
}