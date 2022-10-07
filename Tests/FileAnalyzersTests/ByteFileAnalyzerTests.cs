using System.Text;

namespace Tests.FileAnalyzersTests;

public class ByteFileAnalyzerTests
{
    private readonly string _testText;
    private readonly byte[] _bytes;

    public ByteFileAnalyzerTests()
    {
        _bytes = new byte[] { 0, 1, 2 };

        var textBuilder = new StringBuilder();
        textBuilder
            .Append((char)_bytes[0], 10)
            .Append((char)_bytes[1], 20)
            .Append((char)_bytes[2], 30);

        _testText = textBuilder.ToString();
    }

    [Fact]
    public void GetCountOccurrences_ShouldReturnExpected()
    {
        const string fileName = nameof(GetCountOccurrences_ShouldReturnExpected);
        RemoveFile(fileName);
        CreateTextFile(fileName, _testText);
        var expected = new Dictionary<byte, long>
        {
            { _bytes[0], 10 },
            { _bytes[1], 20 },
            { _bytes[2], 30 }
        };

        var analyzer = new ByteFileAnalyzer(fileName);
        var result = analyzer.GetCountOccurrences();

        RemoveFile(fileName);
        result.Where(item => item.Value > 0)
            .Should().Equal(expected);
    }

    [Fact]
    public void GetProbabilities_ShouldReturnExpected()
    {
        const string fileName = nameof(GetProbabilities_ShouldReturnExpected);
        RemoveFile(fileName);
        CreateTextFile(fileName, _testText);
        var expected = new Dictionary<byte, double>
        {
            { _bytes[0], (double)10 / 60 },
            { _bytes[1], (double)20 / 60 },
            { _bytes[2], (double)30 / 60 }
        };

        var analyzer = new ByteFileAnalyzer(fileName);
        var result = analyzer.GetProbabilities();

        RemoveFile(fileName);
        result.Where(item => item.Value > 0)
            .Should().Equal(expected);
    }

    [Fact]
    public void GetInfoAmountInSymbol_ShouldReturnExpected()
    {
        const string fileName = nameof(GetInfoAmountInSymbol_ShouldReturnExpected);
        RemoveFile(fileName);
        CreateTextFile(fileName, _testText);
        var expected = new Dictionary<byte, double>
        {
            { _bytes[0], -Math.Log2((double)10 / 60) },
            { _bytes[1], -Math.Log2((double)10 / 60) },
            { _bytes[2], -Math.Log2((double)10 / 60) }
        };

        var analyzer = new ByteFileAnalyzer(fileName);
        var result = analyzer.GetInfoAmountInSymbol();

        RemoveFile(fileName);
        result.Where(item => item.Value > 0)
            .Should().Equal(expected);
    }

    private static void CreateTextFile(string fileName, string text)
    {
        File.Create(fileName).Dispose();
        File.WriteAllText(fileName, text);
    }

    private static void RemoveFile(string fileName)
    {
        File.Delete(fileName);
    }
}