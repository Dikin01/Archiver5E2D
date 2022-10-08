using File = Archiver5E2D.Entities.File;
using BitArray = System.Collections.BitArray;
using Archiver5E2D.CompressionAlgorithms;

namespace Archiver5E2D.Compressors;

public class CompressorV2 : Compressor
{
    public override byte Version => 0x3;
    public override byte[] AlgorithmCodes { get; } = { 0, 0, 0 };

    public override File Compress(File file)
    {
        var fileAnalyzer = new ByteFileAnalyzer(file);
        var bytesOccurences = fileAnalyzer.GetCountOccurrences().Where(kvp => kvp.Value > 0).ToList();
        var compressionAlgorithm = new ShennonFanoAlgorithm();
        var codeForBytes = compressionAlgorithm.GetCodeForBytes(bytesOccurences, fileAnalyzer.Length, new BitArray(0));
        AlgorithmCodes[0] = compressionAlgorithm.AlgorithmCode;
        BitArray compressedContent = new BitArray(0);
        foreach (var contentByte in file.Content)
        {
            compressedContent = ConcatTwoBitArrays(compressedContent, codeForBytes[contentByte]);
        }
        return base.Compress(new File(file.Path, file.Name, ConvertBitArrayToByte(compressedContent)));
    }

    private BitArray ConcatTwoBitArrays(BitArray left, BitArray right)
    {
        var bools = new bool[left.Count + right.Count];
        left.CopyTo(bools, 0);
        right.CopyTo(bools, left.Count);
        return new BitArray(bools);
    }

    private byte[] ConvertBitArrayToByte(BitArray bits)
    {
        byte[] bytes = new byte[Convert.ToInt32(Math.Ceiling((double)bits.Length / 8))];
        bits.CopyTo(bytes, 0);
        return bytes;
    }
}