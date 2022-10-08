using BitArray = System.Collections.BitArray;

namespace Archiver5E2D.CompressionAlgorithms;

public abstract class CompressionAlgorithm
{
    public abstract byte AlgorithmCode { get; }

    public abstract Dictionary<byte, BitArray> GetCodeForBytes(List<KeyValuePair<byte, long>> bytesOccurences, long totalLength, BitArray currentCode);

    protected BitArray AddBitToBitArray(BitArray bitArray, bool bit)
    {
        var extendedBitArray = new BitArray(bitArray);
        extendedBitArray.Length += 1;
        extendedBitArray[extendedBitArray.Length - 1] = bit;
        return extendedBitArray;
    }
}