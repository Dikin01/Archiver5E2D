using System.Reflection;
using System.Text;
using Archiver5E2D.FileAnalyzers;
using File = Archiver5E2D.Entities.File;

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
    public void GetCountOccurrences_ShouldReturnExpected_WhenAnalyzerIsCreatedUsingPath()
    {
        var methodName = MethodBase.GetCurrentMethod()!.Name;
        var fileName = nameof(CharFileAnalyzerTests) + methodName;
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
        result.Should().Equal(expected);
    }

    [Fact]
    public void GetCountOccurrences_ShouldReturnExpected_WhenAnalyzerIsCreatedUsingFile()
    {
        var methodName = MethodBase.GetCurrentMethod()!.Name;
        var fileName = nameof(CharFileAnalyzerTests) + methodName;
        CreateTextFile(fileName, _testText);
        var file = File.FromExisting(fileName);
        RemoveFile(fileName);
        var expected = new Dictionary<char, long>
        {
            { _chars[0], 10 },
            { _chars[1], 20 },
            { _chars[2], 30 }
        };

        var analyzer = new CharFileAnalyzer(file);
        var result = analyzer.GetCountOccurrences();

        result.Should().Equal(expected);
    }

    [Fact]
    public void GetProbabilities_ShouldReturnExpected_WhenAnalyzerIsCreatedUsingPath()
    {
        var methodName = MethodBase.GetCurrentMethod()!.Name;
        var fileName = nameof(CharFileAnalyzerTests) + methodName;
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
        result.Should().Equal(expected);
    }

    [Fact]
    public void GetProbabilities_ShouldReturnExpected_WhenAnalyzerIsCreatedUsingFile()
    {
        var methodName = MethodBase.GetCurrentMethod()!.Name;
        var fileName = nameof(CharFileAnalyzerTests) + methodName;
        CreateTextFile(fileName, _testText);
        var file = File.FromExisting(fileName);
        RemoveFile(fileName);
        var expected = new Dictionary<char, double>
        {
            { _chars[0], (double)10 / 60 },
            { _chars[1], (double)20 / 60 },
            { _chars[2], (double)30 / 60 }
        };

        var analyzer = new CharFileAnalyzer(file);
        var result = analyzer.GetProbabilities();

        result.Should().Equal(expected);
    }

    [Fact]
    public void GetInfoAmountInSymbol_ShouldReturnExpected_WhenAnalyzerIsCreatedUsingPath()
    {
        var methodName = MethodBase.GetCurrentMethod()!.Name;
        var fileName = nameof(CharFileAnalyzerTests) + methodName;
        CreateTextFile(fileName, _testText);
        var expected = new Dictionary<char, double>
        {
            { _chars[0], -Math.Log2((double)10 / 60) },
            { _chars[1], -Math.Log2((double)20 / 60) },
            { _chars[2], -Math.Log2((double)30 / 60) }
        };

        var analyzer = new CharFileAnalyzer(fileName);
        var result = analyzer.GetInfoAmountInSymbol();

        RemoveFile(fileName);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetInfoAmountInSymbol_ShouldReturnExpected_WhenAnalyzerIsCreatedUsingFile()
    {
        var methodName = MethodBase.GetCurrentMethod()!.Name;
        var fileName = nameof(CharFileAnalyzerTests) + methodName;
        CreateTextFile(fileName, _testText);
        var file = File.FromExisting(fileName);
        RemoveFile(fileName);
        var expected = new Dictionary<char, double>
        {
            { _chars[0], -Math.Log2((double)10 / 60) },
            { _chars[1], -Math.Log2((double)20 / 60) },
            { _chars[2], -Math.Log2((double)30 / 60) }
        };

        var analyzer = new CharFileAnalyzer(file);
        var result = analyzer.GetInfoAmountInSymbol();

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetInfoAmount_ShouldReturnExpected_WhenAnalyzerIsCreatedUsingPath()
    {
        var methodName = MethodBase.GetCurrentMethod()!.Name;
        var fileName = nameof(CharFileAnalyzerTests) + methodName;
        CreateTextFile(fileName, _testText);
        const double expectedBits = 87.5488;
        const double expectedBytes = 10.9436;

        var analyzer = new CharFileAnalyzer(fileName);
        var result = analyzer.GetInfoAmount();

        RemoveFile(fileName);
        result.bits.Should().BeApproximately(expectedBits, 0.0001);
        result.bytes.Should().BeApproximately(expectedBytes, 0.0001);
    }

    [Fact]
    public void GetInfoAmount_ShouldReturnExpected_WhenAnalyzerIsCreatedUsingFile()
    {
        var methodName = MethodBase.GetCurrentMethod()!.Name;
        var fileName = nameof(CharFileAnalyzerTests) + methodName;
        CreateTextFile(fileName, _testText);
        var file = File.FromExisting(fileName);
        RemoveFile(fileName);
        const double expectedBits = 87.5488;
        const double expectedBytes = 10.9436;

        var analyzer = new CharFileAnalyzer(file);
        var result = analyzer.GetInfoAmount();

        result.bits.Should().BeApproximately(expectedBits, 0.0001);
        result.bytes.Should().BeApproximately(expectedBytes, 0.0001);
    }


    private static void CreateTextFile(string fileName, string text)
    {
        System.IO.File.Create(fileName).Dispose();
        System.IO.File.WriteAllText(fileName, text);
    }

    private static void RemoveFile(string fileName)
    {
        System.IO.File.Delete(fileName);
    }
}