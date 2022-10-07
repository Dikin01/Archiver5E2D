﻿using System.Text;

namespace Archiver5E2D.FileAnalyzers;

public class CharFileAnalyzer : BaseFileAnalyzer<char>
{
    private readonly char[] _chars;
    
    public override long Length => _chars.Length;
    
    public CharFileAnalyzer(string path) : base(path)
    {
        _chars = Encoding.Default.GetChars(_fileBytes);
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