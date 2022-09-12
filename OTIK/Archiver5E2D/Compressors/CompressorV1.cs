using File = Archiver5E2D.Entities.File;

namespace Archiver5E2D.Compressors;

public class CompressorV1 : Compressor
{
    public override byte Version => 0x1;
    public override byte[] AlgorithmCodes => new byte[] { 0, 0, 0 };
}