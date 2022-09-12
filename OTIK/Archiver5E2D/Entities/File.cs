using System.Text;
using static System.IO.Path;
using SystemFile = System.IO.File;

namespace Archiver5E2D.Entities;

public class File
{
    public readonly string Path;
    public readonly string Name;
    public readonly byte[] Content;

    public File(string path, string name, byte[] content)
    {
        // TODO: Добавить исключения
        Path = path;
        Name = name;
        Content = content;
    }

    public static File FromExistingFile(string pathWithFileName)
    {
        var fileInfo = new FileInfo(pathWithFileName);

        if (fileInfo.Exists)
        {
            var path = fileInfo.DirectoryName ?? string.Empty;
            var name = fileInfo.Name;
            var bytes = SystemFile.ReadAllBytes(fileInfo.FullName);

            return new File(path, name, bytes);
        }

        throw new ArgumentException("The path does not lead to the file.");
    }

    public static File Combine(IEnumerable<File> files, string path, string name)
    {
        var resultContent = new List<byte>();
        foreach (var file in files)
        {
            var pathWithFileName = System.IO.Path.Combine(file.Path, file.Name);
            var pathBytes = Encoding.Default.GetBytes(pathWithFileName);
            var pathLength = (uint)pathBytes.Length;

            // Потому что путь файла не может превышать 260 символов
            resultContent.AddRange(BitConverter.GetBytes(pathLength)[..2]);
            resultContent.AddRange(pathBytes);

            var contentLength = (uint)file.Content.Length;
            resultContent.AddRange(BitConverter.GetBytes(contentLength));
            resultContent.AddRange(file.Content);
        }

        return new File(path, name, resultContent.ToArray());
    }

    public static IEnumerable<File> Separate(byte[] bytes)
    {
        var files = new List<File>();

        var index = 0;
        while (index < bytes.Length)
        {
            var list = new List<byte> { bytes[index], bytes[index + 1], 0, 0 };
            var pathLength = BitConverter.ToUInt32(list.ToArray());

            index += 2;
            var pathWithName = Encoding.Default.GetString(bytes, index, (int)pathLength);

            index += (int)pathLength;
            var contentLength = BitConverter.ToUInt32(bytes
                .Skip(index)
                .Take(list.Count)
                .ToArray());

            index += list.Count;
            var content = bytes
                .Skip(index)
                .Take((int)contentLength)
                .ToArray();

            var fileName = GetFileName(pathWithName);
            var path = GetDirectoryName(pathWithName)!;
            files.Add(new File(path, fileName, content));
            index += content.Length;
        }

        return files;
    }

    public void Create()
    {
        var fullPath = System.IO.Path.Combine(Path, Name);
        SystemFile.WriteAllBytes(fullPath, Content);
    }

    public override bool Equals(object? obj)
    {
        if (obj is File file)
            return Equals(file);
        return base.Equals(obj);
    }

    private bool Equals(File other)
    {
        return Path == other.Path && Name == other.Name && Content.SequenceEqual(other.Content);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Path, Name, Content);
    }
}