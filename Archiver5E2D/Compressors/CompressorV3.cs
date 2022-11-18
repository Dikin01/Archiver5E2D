using Archiver5E2D.CompressionAlgorithms;
using File = Archiver5E2D.Entities.File;
using Archiver5E2D.FileAnalyzers;

namespace Archiver5E2D.Compressors;

public class CompressorV3 : Compressor
{
    public const uint DecompressInfoOffset = 12;
    public override uint DataOffset { get; protected set; }
    public override byte Version => 0x3;
    public override byte[] AlgorithmCodes { get; } = { 0, 0, 0 };

    private byte _prefix;

    public override File Compress(File file)
    {
        var fileAnalyzer = new ByteFileAnalyzer(file);

        if (fileAnalyzer.GetCountOccurrences().Count < 1)
            _prefix = 0xAA;
        else
            _prefix = fileAnalyzer
                .GetCountOccurrences()
                .MinBy(pair => pair.Value)
                .Key;

        var rlePrefix = new RlePrefix();
        AlgorithmCodes[1] = 0x1;
        var compressedContent = rlePrefix.Compress(_prefix, file.Content);
        return new File(file.Path, file.Name, AddHeader(compressedContent));
    }

    public override File Decompress(File file)
    {
        byte prefix = file.Content[DecompressInfoOffset];
        DataOffset = DecompressInfoOffset + 1;
        byte[] dataBytes = RemoveHeader(file.Content);
        List<byte> decompressedData = new List<byte>();
        for (int i = 0; i < dataBytes.Length; i++)
        {
            if (dataBytes[i] == prefix)
            {
                byte length = dataBytes[++i];
                length++;
                byte element = length == 1 ? prefix : dataBytes[++i];
                decompressedData.AddRange(Enumerable.Repeat(element, length));
            }
            else
            {
                decompressedData.Add(dataBytes[i]);
            }
        }

        return new File(file.Path, file.Name, decompressedData.ToArray());
    }

    protected override byte[] AddHeader(byte[] contentBytes)
    {
        DataOffset = DecompressInfoOffset + 1;
        var size = (uint)contentBytes.Length;
        var resultLength = size + DataOffset;
        var result = new byte[resultLength];

        for (var i = 0; i < Signature.Length; i++)
            result[i + SignatureOffset] = Signature[i];

        result[VersionOffset] = Version;

        for (var i = 0; i < AlgorithmCodes.Length; i++)
            result[i + AlgorithmCodesOffset] = AlgorithmCodes[i];

        var sizeBytes = BitConverter.GetBytes(size);
        for (var i = 0; i < DataSizeLength; i++)
            result[i + DataSizeOffset] = sizeBytes[i];

        result[DecompressInfoOffset] = _prefix;

        for (var i = DataOffset; i < result.Length; i++)
            result[i] = contentBytes[i - DataOffset];

        return result;
    }
}