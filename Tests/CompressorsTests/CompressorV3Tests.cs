using Archiver5E2D.Compressors;
using File = Archiver5E2D.Entities.File;

namespace Tests.CompressorsTests;

public class CompressorV3Tests
{
    //TODO: Добавить тестов
    private readonly CompressorV3 _compressor = new();
    
    private const string FileName = "FileName";
    private const string Path = @"c:\temp\";
    
    [Fact]
    public void CompressThenDecompress_ShouldReturnEmptyFile_WhenWhenFileIsEmpty()
    {
        var bytes = Array.Empty<byte>();
        var file = new File(Path, FileName, bytes);
        var compressFile = _compressor.Compress(file);

        var decompressFile = _compressor.Decompress(compressFile);

        decompressFile.Should().Be(file);
    }
}