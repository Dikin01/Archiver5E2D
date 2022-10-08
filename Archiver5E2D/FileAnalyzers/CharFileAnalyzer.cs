using System.Text;

namespace Archiver5E2D.FileAnalyzers;

public class CharFileAnalyzer : BaseFileAnalyzer<char>
{
    private readonly char[] _chars;

    protected override IReadOnlyCollection<char> AnalyzedSymbols => _chars;
    protected override long Length => _chars.Length;
    
    public CharFileAnalyzer(string path) : base(path)
    {
        _chars = Encoding.Default.GetChars(FileBytes);
    }
    
    public CharFileAnalyzer(Archiver5E2D.Entities.File file) : base (file)
    {
        _chars = Encoding.Default.GetChars(FileBytes);
    }
}