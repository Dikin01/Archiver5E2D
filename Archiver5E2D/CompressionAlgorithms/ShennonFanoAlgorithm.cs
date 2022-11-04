namespace Archiver5E2D.CompressionAlgorithms;

public class ShennonFanoAlgorithm : CompressionAlgorithm
{
    public override byte AlgorithmCode => 1;

    public override Dictionary<byte, BitsArray> GetCodeForBytes(
        List<KeyValuePair<byte, long>> bytesOccurrences,
        long totalLength,
        BitsArray currentCode)
    {
        if (bytesOccurrences.Count == 1)
            return new Dictionary<byte, BitsArray> { { bytesOccurrences[0].Key, currentCode } };

        bytesOccurrences.Sort((pairA, pairB) => pairA.Value.CompareTo(pairB.Value));
        
        var halfLength = totalLength / 2;
        var leftList = new List<KeyValuePair<byte, long>>();
        long leftSum = 0;
        foreach (var pair in bytesOccurrences)
        {
            leftSum += pair.Value;
            if (leftSum <= halfLength)
                leftList.Add(pair);
            else
                break;
        }

        var rightList = bytesOccurrences.Except(leftList).ToList();
        var leftCode = new BitsArray(currentCode);
        leftCode.AddBit(false);
        
        var rightCode = new BitsArray(currentCode);
        rightCode.AddBit(true);
        
        return GetCodeForBytes(leftList, leftList.Sum(kvp => kvp.Value), leftCode)
            .Concat(GetCodeForBytes(rightList, rightList.Sum(kvp => kvp.Value), rightCode))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}