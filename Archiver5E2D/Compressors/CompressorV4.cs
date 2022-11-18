using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D.Compressors;

public class CompressorV4 : Compressor
{
    private const uint DecompressInfoOffset = 12;
    public override uint DataOffset { get; protected set; }
    public override byte Version => 0x4;
    public override byte[] AlgorithmCodes { get; } = { 0, 0, 0 };

    private const int MaxS = 1023; // 10 bits
    private const int MaxL = 63; // 6 bits

    public override File Compress(File file)
    {
        AlgorithmCodes[1] = 0x2;
        var compressedContent = new List<byte>();
        for (var i = 0; i < file.Content.Length; i++)
        {
            var leftBorder = Math.Max(i - MaxS, 0);
            var bestLen = 0;
            var bestShift = 0;
            for (var j = leftBorder; j < i; j++)
            {
                var rightBorder = Math.Min(i + MaxL, file.Content.Length - 2);
                var currLen = 0;
                while (currLen <= MaxL &&
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

            var firstByte = (byte)((bestLen << 2) | (bestShift >> 8));
            var secondByte = (byte)bestShift;
            i += bestLen;

            compressedContent.Add(firstByte);
            compressedContent.Add(secondByte);
            compressedContent.Add(file.Content[i]);
        }

        var compressedFile = new File(file.Path, file.Name, AddHeader(compressedContent.ToArray()));
        return compressedFile;
    }

    public override File Decompress(File file)
    {
        var dataBytes = RemoveHeader(file.Content);
        var decompressedData = new List<byte>();
        for (var i = 0; i < dataBytes.Length; i++)
        {
            var firstByte = dataBytes[i];
            var secondByte = dataBytes[++i];

            var dataByte = dataBytes[++i];

            var linkLen = (firstByte & 0b11111100) >> 2;
            var linkShift = ((firstByte & 0b00000011) << 8) | secondByte;
            if (linkLen != 0 && linkShift != 0)
            {
                var startIndex = decompressedData.Count - linkShift;
                for (var j = 0; j < linkLen; j++)
                {
                    decompressedData.Add(decompressedData[startIndex + j]);
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