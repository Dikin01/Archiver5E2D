namespace Archiver5E2D.FileAnalyzers;

public abstract class BaseFileAnalyzer<T> where T : notnull
{
    protected readonly byte[] FileBytes;

    public abstract long Length { get; }

    protected abstract IReadOnlyCollection<T> AnalyzedSymbols { get; }

    public BaseFileAnalyzer(string path)
    {
        FileBytes = File.ReadAllBytes(path);
    }

    public BaseFileAnalyzer(Archiver5E2D.Entities.File file)
    {
        FileBytes = file.Content;
    }

    public abstract Dictionary<T, long> GetCountOccurrences();

    public virtual Dictionary<T, double> GetProbabilities()
    {
        var occurrences = GetCountOccurrences();

        return occurrences
            .ToDictionary(occurrence => occurrence.Key,
                occurrence => (double)occurrence.Value / Length);
    }

    public Dictionary<T, double> GetInfoAmountInSymbol()
    {
        var probabilities = GetProbabilities();

        var result = new Dictionary<T, double>();
        foreach (var probability in probabilities)
        {
            if(probability.Value != 0)
                result.Add(probability.Key, -Math.Log2(probability.Value));
            else
                result.Add(probability.Key, 0);
        }

        return result;
    }

    public (double bits, double bytes) GetInfoAmount()
    {
        var infoAmountInSymbol = GetInfoAmountInSymbol();
        var occurrences = GetCountOccurrences();

        var bits = infoAmountInSymbol
            .Where(item => item.Value != 0)
            .Sum(item => item.Value * occurrences[item.Key]);

        var bytes = bits / 8;

        return (bits, bytes);
    }
}