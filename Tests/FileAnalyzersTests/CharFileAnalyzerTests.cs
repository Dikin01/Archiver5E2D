using System.Text;
using Archiver5E2D.FileAnalyzers;

namespace Tests.FileAnalyzersTests;

public class CharFileAnalyzerTests
{
    private readonly string _testText;
    private readonly char[] _chars = { 'a', 'b', 'c' };

    public CharFileAnalyzerTests()
    {
        var textBuilder = new StringBuilder();
        textBuilder
            .Append(_chars[0], 10)
            .Append(_chars[1], 20)
            .Append(_chars[2], 30);

        _testText = textBuilder.ToString();
    }

    [Fact]
    public void GetCountOccurrences_ShouldReturnExpected_WhenCharFileAnalyzer()
    {
        const string fileName = nameof(GetCountOccurrences_ShouldReturnExpected_WhenCharFileAnalyzer);
        RemoveFile(fileName);
        CreateTextFile(fileName, _testText);
        var expected = new Dictionary<char, long>
        {
            { _chars[0], 10 },
            { _chars[1], 20 },
            { _chars[2], 30 }
        };

        var analyzer = new CharFileAnalyzer(fileName);
        var result = analyzer.GetCountOccurrences();

        RemoveFile(fileName);
        result.Where(item => item.Value > 0)
            .Should().Equal(expected);
    }

    [Fact]
    public void GetProbabilities_ShouldReturnExpected_WhenCharFileAnalyzer()
    {
        const string fileName = nameof(GetProbabilities_ShouldReturnExpected_WhenCharFileAnalyzer);
        RemoveFile(fileName);
        CreateTextFile(fileName, _testText);
        var expected = new Dictionary<char, double>
        {
            { _chars[0], (double)10 / 60 },
            { _chars[1], (double)20 / 60 },
            { _chars[2], (double)30 / 60 }
        };

        var analyzer = new CharFileAnalyzer(fileName);
        var result = analyzer.GetProbabilities();

        RemoveFile(fileName);
        result.Where(item => item.Value > 0)
            .Should().Equal(expected);
    }

    [Fact]
    public void GetInfoAmountInSymbol_ShouldReturnExpected_WhenCharFileAnalyzer()
    {
        const string fileName = nameof(GetInfoAmountInSymbol_ShouldReturnExpected_WhenCharFileAnalyzer);
        RemoveFile(fileName);
        CreateTextFile(fileName, _testText);
        var expected = new Dictionary<char, double>
        {
            { _chars[0], -Math.Log2((double)10 / 60) },
            { _chars[1], -Math.Log2((double)20 / 60) },
            { _chars[2], -Math.Log2((double)30 / 60) }
        };

        var analyzer = new CharFileAnalyzer(fileName);
        var result = analyzer.GetInfoAmountInSymbol();

        result.Where(item => item.Value > 0)
            .Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetInfoAmount_ShouldReturnExpected_WhenCharFileAnalyzer()
    {
        const string fileName = nameof(GetInfoAmount_ShouldReturnExpected_WhenCharFileAnalyzer);
        RemoveFile(fileName);
        CreateTextFile(fileName, _testText);
        const double expectedBits = 87.5488;
        const double expectedBytes = 10.9436;

        var analyzer = new CharFileAnalyzer(fileName);
        var result = analyzer.GetInfoAmount();

        result.bits.Should().BeApproximately(expectedBits, 0.0001);
        result.bytes.Should().BeApproximately(expectedBytes, 0.0001);
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