using File = Archiver5E2D.Entities.File;
using Archiver5E2D.CompressionAlgorithms;
using StringBuilder = System.Text.StringBuilder;

namespace Archiver5E2D.Compressors;

public class CompressorV2 : Compressor
{
    public const uint DecompressInfoOffset = 12;
    public override byte Version => 0x2;
    public override byte[] AlgorithmCodes { get; } = { 0, 0, 0 };
    private byte[] DecompressInfo = new byte [0];

    public override File Compress(File file)
    {
        var fileAnalyzer = new ByteFileAnalyzer(file);
        var bytesOccurences = fileAnalyzer.GetCountOccurrences().Where(kvp => kvp.Value > 0).ToList();
        var compressionAlgorithm = new ShennonFanoAlgorithm();
        var codeForBytes = compressionAlgorithm.GetCodeForBytes(bytesOccurences, fileAnalyzer.Length, new BitsArray());
        AlgorithmCodes[0] = compressionAlgorithm.AlgorithmCode;
        DecompressInfo = CreateDecompressInfo(codeForBytes);
        BitsArray compressedContent = new BitsArray();
        foreach (var contentByte in file.Content)
        {
            compressedContent = BitsArray.Concat(compressedContent, codeForBytes[contentByte]);
        }
        File compressedFile = new File(file.Path, file.Name, AddHeader(compressedContent.ToBytes()));
        return compressedFile;
    }

    public override File Decompress(File file)
    {
        var decompressInfo = ReadDecompressInfo(file.Content);
        byte[] dataSizeBytes = new byte[DataSizeLength];
        Array.Copy(file.Content, DataSizeOffset, dataSizeBytes, 0, DataSizeLength);
        uint dataSize = BitConverter.ToUInt32(dataSizeBytes, 0);
        var contentBytes = RemoveHeader(file.Content);
        var bitReader = new ByBitReader(contentBytes);
        var decompressedContent = new List<byte>();
        while(!bitReader.IsContentOver())
        {
            var currentBits = new StringBuilder();
            while (!decompressInfo.ContainsKey(currentBits.ToString()))
            {
                currentBits.Append(bitReader.ReadBits(1));
            }
            decompressedContent.Add(decompressInfo[currentBits.ToString()]);
        }
        return new File(file.Path, file.Name, decompressedContent.ToArray());
    }

    protected override byte[] AddHeader(byte[] contentBytes)
    {
        var decompressInfoLength = (uint)DecompressInfo.Length;
        DataOffset = DecompressInfoOffset + decompressInfoLength;
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

        for (var i = 0; i < decompressInfoLength; i++)
            result[i + DecompressInfoOffset] = DecompressInfo[i];

        for (var i = DataOffset; i < result.Length; i++)
            result[i] = contentBytes[i - DataOffset];

        return result;
    }

    private byte[] CreateDecompressInfo(Dictionary<byte, BitsArray> codeForBytes)
    {
        BitsArray decompressInfo = new BitsArray();
        byte currentByte = 0;
        for (int count = 0; count <= 255; count++, currentByte++)
        {
            if (codeForBytes.ContainsKey(currentByte))
            {
                decompressInfo.AddBits(Convert.ToString(codeForBytes[currentByte].Length(), 2).PadLeft(4, '0'));
                decompressInfo.AddBits(codeForBytes[currentByte].ToString());
            }
            else
            {
                decompressInfo.AddBits("0000");
            }
        }
        return decompressInfo.ToBytes();
    }

    private Dictionary<string, byte> ReadDecompressInfo(byte[] contentBytes)
    {
        Dictionary<string, byte> decompressInfo = new Dictionary<string, byte>();
        var bitReader = new ByBitReader(contentBytes);
        bitReader.currentPos = DecompressInfoOffset;
        byte currentByte = 0;
        for (int count = 0; count <= 255; count++, currentByte++)
        {
            var codeSize = Convert.ToUInt32(bitReader.ReadBits(4), 2);
            if (codeSize == 0) continue;
            decompressInfo[bitReader.ReadBits(codeSize)] = currentByte;
        }
        DataOffset = bitReader.currentPos;
        return decompressInfo;
    }
}