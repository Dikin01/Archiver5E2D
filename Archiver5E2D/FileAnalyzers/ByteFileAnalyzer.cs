using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D.FileAnalyzers;

public class ByteFileAnalyzer : BaseFileAnalyzer<byte>
{
    protected override IReadOnlyCollection<byte> AnalyzedSymbols => FileBytes;
    protected override long Length => FileBytes.Length;

    public ByteFileAnalyzer(string path) : base(path) { }

    public ByteFileAnalyzer(File file) : base(file) { }
}