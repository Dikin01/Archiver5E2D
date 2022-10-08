using Archiver5E2D.FileAnalyzers;

namespace SandBox;

public static class FileAnalyzerExample
{
    public static void Example()
    {
        const string path = @"../../../../SandBox/texts";

        var sumOccurrences = CreateEmptyByteDictionary();

        var fileNames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (var fileName in fileNames)
        { 
            var analyzer = new ByteFileAnalyzer(fileName);
            var tempOccurrences = analyzer.GetCountOccurrences();

            foreach (var occurrence in tempOccurrences)
                sumOccurrences[occurrence.Key] += occurrence.Value;
        }

        Console.WriteLine("Частоты появления октетов в файлах, являющихся простым текстом в различных кодировках");
        foreach (var item in sumOccurrences.OrderByDescending(x => x.Value))
            Console.WriteLine($"{item.Key} - {item.Value}");

        Console.WriteLine("4 наиболее частых октета среди всех используемых");
        foreach (var item in sumOccurrences.OrderByDescending(x => x.Value).Take(4))
            Console.WriteLine($"{item.Key} - {item.Value}");

        Console.WriteLine("4 наиболее частых октета, не являющихся кодами печатных символов ASCII");
        var nonPrintableCharacters = sumOccurrences
            .Where(x => x.Key is 127 or < 32)
            .OrderByDescending(x => x.Value)
            .Take(4);
        foreach (var item in nonPrintableCharacters)
            Console.WriteLine($"{item.Key} - {item.Value}");
    }
    
    private static Dictionary<byte, long> CreateEmptyByteDictionary()
    {
        var result = new Dictionary<byte, long>();
        for (byte i = 0; i < 255; i++)
            result.Add(i, 0);
        result.Add(255, 0);

        return result;
    }
}