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

        for (var i = char.MinValue; i < char.MaxValue; i++)
            result.Add(i, 0);
        result.Add(char.MaxValue, 0);

        foreach (var symbol in _chars)
            result[symbol]++;

        return result;
    }
}