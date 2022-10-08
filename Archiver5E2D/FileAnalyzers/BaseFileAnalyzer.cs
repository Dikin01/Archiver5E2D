namespace Archiver5E2D.FileAnalyzers;

public abstract class BaseFileAnalyzer<T> where T : notnull
{
    protected readonly byte[] FileBytes;

    protected abstract IReadOnlyCollection<T> AnalyzedSymbols { get; }
    protected abstract long Length { get; }

    protected BaseFileAnalyzer(string path)
    {
        FileBytes = File.ReadAllBytes(path);
    }

    protected BaseFileAnalyzer(Archiver5E2D.Entities.File file)
    {
        FileBytes = file.Content;
    }

    public Dictionary<T, long> GetCountOccurrences()
    {
        var result = new Dictionary<T, long>();

        foreach (var symbol in AnalyzedSymbols)
            result[symbol] = result.GetValueOrDefault(symbol, 0) + 1;

        return result;
    }

    public Dictionary<T, double> GetProbabilities()
    {
        var occurrences = GetCountOccurrences();

        return occurrences
            .ToDictionary(occurrence => occurrence.Key,
                occurrence => (double)occurrence.Value / Length);
    }

    public Dictionary<T, double> GetInfoAmountInSymbol()
    {
        var probabilities = GetProbabilities();

        return probabilities
            .ToDictionary(probability => probability.Key,
                probability => -Math.Log2(probability.Value));
    }

    public (double bits, double bytes) GetInfoAmount()
    {
        var infoAmountInSymbol = GetInfoAmountInSymbol();
        var occurrences = GetCountOccurrences();

        var bits = infoAmountInSymbol
            .Sum(item => item.Value * occurrences[item.Key]);

        var bytes = Math.Ceiling(bits);

        return (bits, bytes);
    }
}