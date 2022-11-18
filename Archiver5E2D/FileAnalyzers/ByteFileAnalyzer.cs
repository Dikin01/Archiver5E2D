namespace Archiver5E2D.FileAnalyzers;

public class ByteFileAnalyzer : BaseFileAnalyzer<byte>
{
    public ByteFileAnalyzer(string path) : base(path)
    {
    }

    public ByteFileAnalyzer(Archiver5E2D.Entities.File file) : base(file)
    {
    }

    public override long Length => FileBytes.Length;
    protected override IReadOnlyCollection<byte> AnalyzedSymbols => FileBytes;

    public override Dictionary<byte, long> GetCountOccurrences()
    {
        var result = new Dictionary<byte, long>();

        foreach (var symbol in AnalyzedSymbols)
        {
            if (result.ContainsKey(symbol))
                result[symbol]++;
            else
                result[symbol] = 1;
        }

        return result;
    }
}