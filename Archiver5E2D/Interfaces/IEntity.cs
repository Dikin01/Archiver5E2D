namespace Archiver5E2D.Interfaces;

public interface IEntity
{
    public string Path { get; }
    public string Name { get; }
    public byte[] Content { get; }
    public TypeEntity Type { get; }

    public void Create(string path);
    
    public enum TypeEntity
    {
        File = 0,
        Folder = 1
    }
}