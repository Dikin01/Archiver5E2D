using Archiver5E2D.FileAnalyzers;

namespace SandBox;

public static class MarkovAnalyzerExample
{
    const string path = @"../../../../SandBox/texts/2.txt";

    public static void Example()
    {
        var analyzer = new MarkovAnalyzer(path);

        var occurrences = analyzer.GetCountOccurrences();
        var result = occurrences.ToList().OrderByDescending(occurrence => occurrence.Value);

        Console.WriteLine("Length {0}", analyzer.Length);
        foreach (var occurrence in result)
        {
            Console.WriteLine($"({occurrence.Key.Item1}, {occurrence.Key.Item2}) = {occurrence.Value}");
        }
        Console.WriteLine("Probabilities\n\n");
        var probabilities = analyzer.GetProbabilities();
        var result1 = probabilities.ToList()
            .OrderByDescending(probability => probability.Value);
        foreach (var probability in result1)
        {
            Console.WriteLine($"({probability.Key.Item1}, {probability.Key.Item2}) = {probability.Value}");
        }

        var fileInfo = analyzer.GetInfoAmount();
        Console.WriteLine("File info in bits = {0}, bytes = {1}", fileInfo.bits, fileInfo.bytes);

    }
}