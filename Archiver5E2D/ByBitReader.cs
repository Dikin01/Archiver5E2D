using System.Text;

namespace Archiver5E2D;

public class ByBitReader
{
    private readonly byte[] _content;
    private StringBuilder _readBits = new();

    public uint CurrentPosition { get; set; }

    public ByBitReader(byte[] content)
    {
        _content = content;
    }

    public string ReadBits(uint count)
    {
        var bits = new StringBuilder();
        for (var i = 0; i < count; i++)
        {
            if (_readBits.Length == 0)
                _readBits = new StringBuilder(Convert.ToString(_content[CurrentPosition++], 2)
                    .PadLeft(8, '0'));
            bits.Append(_readBits[^1]);
            _readBits.Length--;
        }

        return bits.ToString();
    }

    public bool IsContentOver()
    {
        return CurrentPosition >= _content.Length;
    }
}