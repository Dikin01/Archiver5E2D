using System.Text;

namespace Archiver5E2D.FileAnalyzers;

public class CharFileAnalyzer : BaseFileAnalyzer<char>
{
    private readonly char[] _chars;

    protected override IReadOnlyCollection<char> AnalyzedSymbols => _chars;
    public override long Length => _chars.Length;

    public CharFileAnalyzer(string path) : base(path)
    {
        _chars = Encoding.Default.GetChars(FileBytes);
    }

    public CharFileAnalyzer(Archiver5E2D.Entities.File file) : base(file)
    {
        _chars = Encoding.Default.GetChars(FileBytes);
    }

    public override Dictionary<char, long> GetCountOccurrences()
    {
        var result = new Dictionary<char, long>();

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