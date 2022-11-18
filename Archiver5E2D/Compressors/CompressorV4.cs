using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D.Compressors;

public class CompressorV4 : Compressor
{
    public const uint DecompressInfoOffset = 12;
    public override uint DataOffset { get; protected set; }
    public override byte Version => 0x4;
    public override byte[] AlgorithmCodes { get; } = { 0, 0, 0 };
    private const int _maxS = 1023; // 10 bits
    private const int _maxL = 63; // 6 bits

    public override File Compress(File file)
    {
        AlgorithmCodes[1] = 0x2;
        List<byte> compressedContent = new List<byte>();
        for (int i = 0; i < file.Content.Count(); i++)
        {
            int leftBorder = Math.Max(i - _maxS, 0);
            int bestLen = 0;
            int bestShift = 0;
            for (int j = leftBorder; j < i; j++)
            {
                int rightBorder = Math.Min(i + _maxL, file.Content.Count() - 1);
                int currLen = 0;
                while (currLen <= _maxL &&
                    i + currLen <= rightBorder &&
                    file.Content[i + currLen] == file.Content[j + currLen])
                {
                    currLen++;
                }
                if (currLen > bestLen) 
                {
                    bestLen = currLen;
                    bestShift = i - j;
                }
            }
            byte firstbyte = (byte)((bestLen << 2) | (bestShift >> 8));
            byte secondbyte = (byte)bestShift;
            i += bestLen;
            compressedContent.Add(firstbyte);
            compressedContent.Add(secondbyte);
            compressedContent.Add(file.Content[i]);
        }
        File compressedFile = new File(file.Path, file.Name, AddHeader(compressedContent.ToArray()));
        return compressedFile;
    }

    public override File Decompress(File file)
    {
        byte[] dataBytes = RemoveHeader(file.Content);
        List<byte> decompressedData = new List<byte>();
        for (int i = 0; i < dataBytes.Length; i++)
        {
            byte firstbyte = dataBytes[i];
            byte secondbyte = dataBytes[++i];
            byte dataByte = dataBytes[++i];
            int linkLen = (firstbyte & 0b11111100) >> 2;
            int linkShift = ((firstbyte & 0b00000011) << 8) | secondbyte;
            if (linkLen != 0 && linkShift != 0)
            {
                for (int j = 0; j < linkLen; j++)
                {
                    decompressedData.Add(decompressedData[(i - linkShift) + j]);
                }
            }
            decompressedData.Add(dataByte);
        }
        return new File(file.Path, file.Name, decompressedData.ToArray());
    }

    protected override byte[] AddHeader(byte[] contentBytes)
    {
        DataOffset = DecompressInfoOffset;
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

        for (var i = DataOffset; i < result.Length; i++)
            result[i] = contentBytes[i - DataOffset];

        return result;
    }
}