using Archiver5E2D.Compressors;
using File = Archiver5E2D.Entities.File;

namespace Tests.CompressorsTests;

public class CompressorV4Tests
{
    //TODO: Добавить тестов
    private readonly CompressorV4 _compressor = new();

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

    [Theory]
    [InlineData(new byte[] { 1, 1, 1 })]
    [InlineData(new byte[] { 1, 1, 1, 1, 2, 3, 4, 1, 1, 1, 1 })]
    public void CompressThenDecompress_ShouldReturnEmptyFile_WhenWhenFileIsNotEmpty(byte[] data)
    {
        var file = new File(Path, FileName, data);
        var compressFile = _compressor.Compress(file);

        var decompressFile = _compressor.Decompress(compressFile);

        decompressFile.Should().Be(file);
    }
}