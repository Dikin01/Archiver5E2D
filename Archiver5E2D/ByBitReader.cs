using StringBuilder = System.Text.StringBuilder;

namespace Archiver5E2D;

public class ByBitReader
{
    public byte[] content { get; }

    public uint currentPos { get; set;}

    private StringBuilder readBits = new StringBuilder();

    public ByBitReader(byte[] content)
    {
        this.content = content;
    }

    public string ReadBits(uint count)
    {
        var bits = new StringBuilder();
        for (int i = 0; i < count; i++)
        {
            if (readBits.Length == 0)
            {
                readBits = new StringBuilder(Convert.ToString(content[currentPos++], 2).PadLeft(8, '0'));
            }
            bits.Append(readBits[readBits.Length - 1]);
            readBits.Length--;
        }
        return bits.ToString();
    }

    public bool IsContentOver()
    {
        return currentPos >= content.Length;
    }
}