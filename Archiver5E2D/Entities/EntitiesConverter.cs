using System.Text;
using Archiver5E2D.Interfaces;
using static System.IO.Path;

namespace Archiver5E2D.Entities;

public static class EntitiesConverter
{
    private static readonly IDictionary<byte, Func<string, string, byte[], IEntity>> 
        FactoryFunctionsForEntitiesBySupportedTypeId = 
            new Dictionary<byte, Func<string, string, byte[], IEntity>>
    {
        { File.GetTypeId(), (path, name, content) => new File(path, name, content) },
        { Folder.GetTypeId(), (path, name, content) => new Folder(path, name,
            ConvertToEntities(content).ToList()) }
    };

    public static File CombineToFile(IEnumerable<IEntity> entities, string path, string name)
    {
        var resultContent = new List<byte>();
        foreach (var entity in entities)
        {
            resultContent.Add(entity.TypeId);

            var pathWithName = Combine(entity.Path, entity.Name);
            var pathBytes = Encoding.Default.GetBytes(pathWithName);
            var pathLength = (ushort)pathBytes.Length;

            // Потому что путь не может превышать 260 символов
            resultContent.AddRange(BitConverter.GetBytes(pathLength));
            resultContent.AddRange(pathBytes);

            var contentLength = (uint)entity.Content.Length;
            resultContent.AddRange(BitConverter.GetBytes(contentLength));
            resultContent.AddRange(entity.Content);
        }

        return new File(path, name, resultContent.ToArray());
    }

    public static IEnumerable<IEntity> SplitIntoEntities(File combinedFile)
    {
        return ConvertToEntities(combinedFile.Content);
    }

    private static IEnumerable<IEntity> ConvertToEntities(byte[] bytes)
    {
        var entities = new List<IEntity>();

        var index = 0;
        while (index < bytes.Length)
        {
            var typeId = bytes[index];
            CheckTypeId(typeId);
            index++;

            var pathLengthBytes = new List<byte> { bytes[index], bytes[index + 1] };
            var pathLength = BitConverter.ToUInt16(pathLengthBytes.ToArray());
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

            byte[] entityContent = bytes
                .Skip(index)
                .Take((int)contentLength)
                .ToArray();
            index += entityContent.Length;
            
            var createEntity = FactoryFunctionsForEntitiesBySupportedTypeId[typeId];
            var entity = createEntity(entityPath, entityName, entityContent);
            entities.Add(entity);
        }

        return entities;
    }

    private static void CheckTypeId(byte typeId)
    {
        if (!IsSupportedType(typeId))
        {
            throw new Exception($"A typeId must be equal to " +
                                $"{string.Join(", ", FactoryFunctionsForEntitiesBySupportedTypeId.Keys)}");
        }
    }
    
    private static bool IsSupportedType(byte typeId) => 
        FactoryFunctionsForEntitiesBySupportedTypeId.ContainsKey(typeId);
}