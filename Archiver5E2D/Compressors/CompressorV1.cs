namespace Archiver5E2D.Compressors;

public class CompressorV1 : Compressor
{
    public override uint DataOffset { get; protected set; } = 12;
    public override byte Version => 0x1;
    public override byte[] AlgorithmCodes { get; } = { 0, 0, 0 };
}