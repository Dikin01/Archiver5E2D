using Archiver5E2D.Compressors;
using File = Archiver5E2D.Entities.File;

namespace Tests.CompressorsTests;

public class CompressorV1Tests
{
    private readonly CompressorV1 _compressor = new();

    private const string FileName = "FileName";
    private const string Path = @"c:\temp\";

    [Fact]
    public void Compress_ShouldContain12Bytes_WhenFileIsEmpty()
    {
        var bytes = Array.Empty<byte>();
        var file = new File(Path, FileName, bytes);

        var result = _compressor.Compress(file);

        result.Content.Should().HaveCount(12);
        var headerBytes = result.Content.Take((int)Compressor.DataOffset).ToList();
        TestHeader(headerBytes, (uint)bytes.Length);
    }

    [Fact]
    public void Compress_ShouldReturn13Bytes_WhenFileDoesContainOneByte()
    {
        var bytes = new byte[] { 0 };
        var file = new File(Path, FileName, bytes);

        var result = _compressor.Compress(file);

        result.Content.Should().HaveCount(13);
        var headerBytes = result.Content.Take((int)Compressor.DataOffset).ToList();
        TestHeader(headerBytes, (uint)bytes.Length);
    }
    
    [Fact]
    public void CompressThenDecompress_ShouldReturnEmptyFile_WhenWhenFileIsEmpty()
    {
        var bytes = Array.Empty<byte>();
        var file = new File(Path, FileName, bytes);
        var compressFile = _compressor.Compress(file);

        var decompressFile = _compressor.Decompress(compressFile);

        decompressFile.Should().Be(file);
    }
    
    [Fact]
    public void CompressThenDecompress_ShouldReturnOriginFile_WhenFileIsNotEmpty()
    {
        // TEST BRANCH RULE AND ACTIONS
        var bytes = Array.Empty<byte>()
        var file = new File(Path, FileName, bytes);
        var compressFile = _compressor.Compress(file);

        var decompressFile = _compressor.Decompress(compressFile);

        decompressFile.Should().Be(file);
    }
    
    [Fact]
    public void Compress_ShouldThrowArgumentException_WhenFileIsNull()
    {
        var act = () => _compressor.Compress(null);

        act.Should().Throw<NullReferenceException>();
    }
    
    [Fact]
    public void Decompress_ShouldThrowArgumentException_WhenFileIsNull()
    {
        var act = () => _compressor.Decompress(null);

        act.Should().Throw<NullReferenceException>();
    }

    private void TestHeader(IList<byte> header, uint expectedContentSize)
    {
        header.Should().Contain(_compressor.Signature);
        header.Should().Contain(_compressor.Version);
        header.Should().Contain(_compressor.AlgorithmCodes);
        TestContentSize(expectedContentSize,
            header
                .Skip((int)Compressor.DataSizeOffset)
                .Take((int)Compressor.DataSizeLength)
                .ToArray());
    }

    private static void TestContentSize(uint expectedSize, byte[] sizeBytes)
    {
        var result = BitConverter.ToUInt32(sizeBytes);
        result.Should().Be(expectedSize);
    }
}