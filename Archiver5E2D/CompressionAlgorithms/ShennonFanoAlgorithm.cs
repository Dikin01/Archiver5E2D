using File = Archiver5E2D.Entities.File;
using BitArray = System.Collections.BitArray;

namespace Archiver5E2D.CompressionAlgorithms;

public class ShennonFanoAlgorithm : CompressionAlgorithm
{
    public override byte AlgorithmCode { get; } = 1;
    
    public override Dictionary<byte, BitArray> GetCodeForBytes(List<KeyValuePair<byte, long>> bytesOccurences, long totalLength, BitArray currentCode)
    {
        if (bytesOccurences.Count == 1)
        {
            return new Dictionary<byte, BitArray>(){{bytesOccurences[0].Key, currentCode}};
        }
        bytesOccurences.Sort(
            delegate(KeyValuePair<byte, long> pair1, KeyValuePair<byte, long> pair2)
            {
                return pair1.Value.CompareTo(pair2.Value);
            }
        );
        var halfLength = totalLength / 2;
        var leftList = new List<KeyValuePair<byte, long>>();
        long leftSum = 0;
        foreach (var pair in bytesOccurences)
        {
            leftSum += pair.Value;
            if (leftSum <= halfLength)
            {
                leftList.Add(pair);
            }
            else
            {
                break;
            }
        }
        var rightList = bytesOccurences.Except(leftList).ToList();
        return GetCodeForBytes(leftList, leftList.Sum(kvp => kvp.Value), AddBitToBitArray(currentCode, false))
        .Concat(GetCodeForBytes(rightList, rightList.Sum(kvp => kvp.Value), AddBitToBitArray(currentCode, true)))
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}