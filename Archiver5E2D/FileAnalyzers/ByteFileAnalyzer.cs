namespace Archiver5E2D.FileAnalyzers;

public class ByteFileAnalyzer : BaseFileAnalyzer<byte>
{
    public ByteFileAnalyzer(string path) : base(path) { }
    public ByteFileAnalyzer(Archiver5E2D.Entities.File file) : base(file) { }

    public override long Length => FileBytes.Length;

    public override Dictionary<byte, long> GetCountOccurrences()
    {
        var result = new Dictionary<byte, long>();

        for (byte i = 0; i < 255; i++)
            result.Add(i, 0);
        result.Add(255, 0);

        foreach (var symbol in FileBytes)
            result[symbol]++;

        return result;
    }
}