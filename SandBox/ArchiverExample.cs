using Archiver5E2D;

namespace SandBox;

public static class ArchiverExample
{
    private const string Path = @"../../../../SandBox/texts/Керниган, Ричи. Язык C — utf8.txt";
    
    public static void Example()
    {
        var compressedFile = Archiver.Archive(Path);
        compressedFile.Save("/ArchiverExample");

        var decompressedFile = Archiver.Dearchive("/ArchiverExample/Керниган, Ричи. Язык C — utf8.5e2d");
        foreach (var file in decompressedFile)
            file.Save("/ArchiverExample");
        
    }
}