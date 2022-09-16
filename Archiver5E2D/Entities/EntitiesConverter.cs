using System.Text;
using Archiver5E2D.Interfaces;
using static System.IO.Path;

namespace Archiver5E2D.Entities;

public static class EntitiesConverter
{
    private static class EntityFactory
    {
        public static IEntity Create(byte typeId, string path, string name, byte[] content)
        {
            if (typeId == File.GetTypeId()) return new File(path, name, content);
            if (typeId == Folder.GetTypeId()) return new Folder(path, name, ConvertToEntities(content));
            throw new InvalidOperationException($"Entity with typeId = {typeId} is can't be created");
        }
    }

    public static File CombineToFile(IEnumerable<IEntity> entities, string path, string name)
    {
        var resultContent = new List<byte>();
        foreach (var entity in entities)
        {
            var pathWithName = Combine(entity.Path, entity.Name);
            var pathBytes = Encoding.Default.GetBytes(pathWithName);
            var pathLength = (ushort)pathBytes.Length; // Потому что путь не может превышать 260 символов
            var contentLength = (uint)entity.Content.Length;
            
            resultContent.Add(entity.TypeId);
            resultContent.AddRange(BitConverter.GetBytes(pathLength));
            resultContent.AddRange(pathBytes);
            resultContent.AddRange(BitConverter.GetBytes(contentLength));
            resultContent.AddRange(entity.Content);
        }

        return new File(path, name, resultContent.ToArray());
    }

    public static IEnumerable<IEntity> SplitIntoEntities(File combinedFile)
    {
        return ConvertToEntities(combinedFile.Content);
    }

    private static List<IEntity> ConvertToEntities(byte[] bytes)
    {
        var entities = new List<IEntity>();

        var index = 0;
        while (index < bytes.Length)
        {
            var typeId = bytes[index];
            index++;

            var pathLengthBytes = new[] { bytes[index], bytes[index + 1] };
            var pathLength = BitConverter.ToUInt16(pathLengthBytes);
            index += 2;

            var pathWithName = Encoding.Default.GetString(bytes, index, pathLength);
            var entityName = GetFileName(pathWithName);
            var entityPath = GetDirectoryName(pathWithName)!;
            index += pathLength;

            const int contentLengthSize = sizeof(uint);
            var contentLength = BitConverter.ToUInt32(bytes
                .Skip(index)
                .Take(contentLengthSize)
                .ToArray());
            index += contentLengthSize;

            var entityContent = bytes
                .Skip(index)
                .Take((int)contentLength)
                .ToArray();
            index += entityContent.Length;
            
            var entity = EntityFactory.Create(typeId, entityPath, entityName, entityContent);
            entities.Add(entity);
        }

        return entities;
    }
}