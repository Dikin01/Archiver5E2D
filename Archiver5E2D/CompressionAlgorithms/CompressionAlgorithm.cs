namespace Archiver5E2D.CompressionAlgorithms;

public abstract class CompressionAlgorithm
{
    public abstract byte AlgorithmCode { get; }

    public abstract Dictionary<byte, BitsArray> GetCodeForBytes(List<KeyValuePair<byte, long>> bytesOccurences, long totalLength, BitsArray currentCode);
}