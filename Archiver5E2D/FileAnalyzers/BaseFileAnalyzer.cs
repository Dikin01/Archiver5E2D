namespace Archiver5E2D.FileAnalyzers;

public abstract class BaseFileAnalyzer<T> where T : notnull
{
    protected readonly byte[] _fileBytes;

    public abstract long Length { get; }

    public BaseFileAnalyzer(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("File not found", Path.GetFileName(path));

        _fileBytes = File.ReadAllBytes(path);
    }

    public abstract Dictionary<T, long> GetCountOccurrences();

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
            .Where(item => item.Value != 0)
            .Sum(item => item.Value * occurrences[item.Key]);

        var bytes = Math.Ceiling(bits);

        return (bits, bytes);
    }
}