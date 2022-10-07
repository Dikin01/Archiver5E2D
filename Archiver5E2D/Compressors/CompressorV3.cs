using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D.Compressors;

public class CompressorV3 : Compressor
{
    public override byte Version => 0x3;
    public override byte[] AlgorithmCodes { get; } = { 1, 0, 0 };

    public override File Compress(File file)
    {
        var fileAnalyzer = new ByteFileAnalyzer(file);
        var bytesOccurences = fileAnalyzer.GetCountOccurrences().Where(kvp => kvp.Value > 0).ToList();
        var bytesCodes = CodesCalculation(bytesOccurences, fileAnalyzer.Length, "");
        return base.Compress(file);
    }

    private Dictionary<byte, string> CodesCalculation(List<KeyValuePair<byte, long>> bytesOccurences, long totalLength, string currentCode)
    {
        if (bytesOccurences.Count == 1)
        {
            return new Dictionary<byte, string>(){{bytesOccurences[0].Key, currentCode}};
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
        return CodesCalculation(leftList, leftList.Sum(kvp => kvp.Value), currentCode + "0")
        .Concat(CodesCalculation(rightList, rightList.Sum(kvp => kvp.Value), currentCode + "1"))
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}