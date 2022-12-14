using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D.FileAnalyzers;

public class MarkovAnalyzer : BaseFileAnalyzer<(byte, byte)>
{
    private readonly HashSet<(byte, byte)> _pairs;
    
    public MarkovAnalyzer(string path) : base(path)
    {
        _pairs = GetPairs();
    }

    public MarkovAnalyzer(File file) : base(file)
    {
        _pairs = GetPairs();
    }

    protected override IReadOnlyCollection<(byte, byte)> AnalyzedSymbols => _pairs;
    public override long Length => FileBytes.Length;

    public override Dictionary<(byte, byte), long> GetCountOccurrences()
    {
        var result = new Dictionary<(byte, byte), long>();
        for (var i = 0; i < FileBytes.Length - 1; i++)
            if (result.ContainsKey((FileBytes[i], FileBytes[i + 1])))
                result[(FileBytes[i], FileBytes[i + 1])]++;
            else
                result[(FileBytes[i], FileBytes[i + 1])] = 1;

        return result;
    }

    public override Dictionary<(byte, byte), double> GetProbabilities()
    {
        var unconditionalProbabilities = base.GetProbabilities();

        var keys = unconditionalProbabilities.Keys;

        var firstSymbols = keys
            .Select(key => key.Item1)
            .Distinct()
            .ToArray();

        var firstSymbolsUnconditionalProbabilities = firstSymbols
            .ToDictionary(symbol => symbol,
                symbol => unconditionalProbabilities
                    .Where(pair => pair.Key.Item1 == symbol)
                    .Sum(item => item.Value));

        return unconditionalProbabilities
            .ToDictionary(probability => probability.Key,
                probability => probability.Value / firstSymbolsUnconditionalProbabilities
                    .Single(item => item.Key == probability.Key.Item1).Value);
    }
    
    private HashSet<(byte, byte)> GetPairs()
    {
        var pairs = new HashSet<(byte, byte)>();

        for (var i = 0; i < FileBytes.Length - 1; i++)
            pairs.Add((FileBytes[i], FileBytes[i + 1]));

        return pairs;
    }
}