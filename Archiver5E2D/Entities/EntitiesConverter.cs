using System.Text;
using Archiver5E2D.Interfaces;
using static System.IO.Path;

namespace Archiver5E2D.Entities;

public static class EntitiesConverter
{
    public static File Combine(IEnumerable<IEntity> entities, string path, string name)
    {
        var resultContent = new List<byte>();
        foreach (var entity in entities)
        {
            resultContent.Add((byte)entity.Type);
            
            var pathWithName = Path.Combine(entity.Path, entity.Name);
            var pathBytes = Encoding.Default.GetBytes(pathWithName);
            var pathLength = (ushort)pathBytes.Length;

            // Потому что путь не может превышать 260 символов
            resultContent.AddRange(BitConverter.GetBytes(pathLength)[..2]);
            resultContent.AddRange(pathBytes);

            var contentLength = (uint)entity.Content.Length;
            resultContent.AddRange(BitConverter.GetBytes(contentLength));
            resultContent.AddRange(entity.Content);
        }

        return new File(path, name, resultContent.ToArray());
    }

    public static IEnumerable<IEntity> Separate(byte[] bytes)
    {
        var entities = new List<IEntity>();

        var index = 0;
        while (index < bytes.Length)
        {
            var typeByte = bytes[index];
            var type = (IEntity.TypeEntity)typeByte;
            // Вынести в отдельную функцию
            if (type != IEntity.TypeEntity.File && type != IEntity.TypeEntity.Folder)
                throw new Exception($"A byte of type must be equal to 0x{IEntity.TypeEntity.File:X} or" +
                                    $" 0x{IEntity.TypeEntity.Folder:X}");
            index++;
            
            var list = new List<byte> { bytes[index], bytes[index + 1], 0, 0 };
            var pathLength = BitConverter.ToUInt32(list.ToArray());
            index += 2;
            
            var pathWithName = Encoding.Default.GetString(bytes, index, (int)pathLength);
            var fileName = GetFileName(pathWithName);
            var path = GetDirectoryName(pathWithName)!;
            index += (int)pathLength;
            
            var contentLength = BitConverter.ToUInt32(bytes
                .Skip(index)
                .Take(list.Count)
                .ToArray());
            index += list.Count;

            byte[] content;
            IEntity entity;
            if (type == IEntity.TypeEntity.File)
            {
                content = bytes
                    .Skip(index)
                    .Take((int)contentLength)
                    .ToArray();
                entity = new File(path, fileName, content);
            }
            else
            {
                content = bytes
                    .Skip(index)
                    .Take((int)contentLength)
                    .ToArray();
                var entitiesEntity = Separate(content);
                entity = new Folder(path, fileName, entitiesEntity.ToList());
            }

            entities.Add(entity);
            index += content.Length;
        }

        return entities;
    }
}