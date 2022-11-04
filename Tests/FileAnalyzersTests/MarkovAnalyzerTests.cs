using File = Archiver5E2D.Entities.File;
using Archiver5E2D.FileAnalyzers;

namespace Tests.FileAnalyzersTests;

public class MarkovAnalyzerTests
{
    [Fact]
    public void Test()
    {
        var content = new byte[] { 0, 0, 1, 1, 0, 1 };
        var file = new File("", "name", content);
        var analyzer = new MarkovAnalyzer(file);

        analyzer.GetCountOccurrences();
        analyzer.GetProbabilities();
        analyzer.GetInfoAmountInSymbol();
        var result = analyzer.GetInfoAmount();
    }
}