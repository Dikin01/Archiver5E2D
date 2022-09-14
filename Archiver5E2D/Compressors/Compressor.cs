using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D.Compressors;

public abstract class Compressor
{
    public readonly byte[] Signature = { 0x5E, 0x2D, 0x5E, 0x2D };
    public const byte VersionLength = 1;
    public const uint DataSizeLength = 4;
    
    public const uint SignatureOffset = 0;
    public const uint VersionOffset = 4;
    public const uint AlgorithmCodesOffset = 5;
    public const uint DataSizeOffset = 8;
    public const uint DataOffset = 12;
    
    public abstract byte Version { get; }
    public abstract byte[] AlgorithmCodes { get; }

    public virtual File Compress(File file)
    {
        var contentBytes = AddHeader(file.Content);
        return new File(file.Path, file.Name, contentBytes);
    }

    public virtual File Decompress(File file)
    {
        var contentBytes = RemoveHeader(file.Content);
        return new File(file.Path, file.Name, contentBytes);
    }
    
    private byte[] AddHeader(byte[] contentBytes)
    {
        // TODO: Имеем ограничение на размер в 2 GB, надо исправлять на чанки
        var size = (uint)contentBytes.Length;
        var resultLength = size + Signature.Length + VersionLength + AlgorithmCodes.Length + DataSizeLength;
        var result = new byte[resultLength];

        for (var i = 0; i < Signature.Length; i++)
            result[i + SignatureOffset] = Signature[i];

        result[VersionOffset] = Version;

        for (var i = 0; i < AlgorithmCodes.Length; i++)
            result[i + AlgorithmCodesOffset] = AlgorithmCodes[i];

        var sizeBytes = BitConverter.GetBytes(size);
        for (var i = 0; i < DataSizeLength; i++)
            result[i + DataSizeOffset] = sizeBytes[i];

        for (var i = DataOffset; i < result.Length; i++)
            result[i] = contentBytes[i - DataOffset];

        return result;
    }
    
    private byte[] RemoveHeader(byte[] contentBytes)
    {
        var result = contentBytes.Skip((int)DataOffset).ToArray();

        return result;
    }
}