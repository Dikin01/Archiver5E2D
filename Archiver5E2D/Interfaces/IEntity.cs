namespace Archiver5E2D.Interfaces;

public interface IEntity
{
    string Path { get; }
    string Name { get; }
    byte[] Content { get; }
    byte TypeId { get; }

    void Create(string path);
}