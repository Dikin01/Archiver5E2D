﻿using Archiver5E2D.Interfaces;
using SystemFile = System.IO.File;

namespace Archiver5E2D.Entities;

public class File : IEntity
{
    public string Path { get; }
    public string Name { get; }
    public byte[] Content { get; }
    public IEntity.TypeEntity Type => IEntity.TypeEntity.File;

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

    public void Create(string rootPath)
    {
        // TODO: Добавить тест на создание пути, если он не существует
        Directory.CreateDirectory(rootPath);
        var fullPath = System.IO.Path.Combine(rootPath, Name);
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
}