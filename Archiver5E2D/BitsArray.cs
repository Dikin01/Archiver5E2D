using System.Collections;
using System.Text;

namespace Archiver5E2D;

public class BitsArray
{
    private BitArray _bitArray = new(0);

    public int Length => _bitArray.Length;

    public BitsArray() { }

    public BitsArray(BitArray bitArray)
    {
        _bitArray = bitArray;
    }

    public BitsArray(BitsArray bitsArray)
    {
        _bitArray = new BitArray(bitsArray._bitArray);
    }

    public void AddBit(bool bit)
    {
        var extendedBitArray = new BitArray(_bitArray);
        extendedBitArray.Length += 1;
        extendedBitArray[^1] = bit;
        _bitArray = extendedBitArray;
    }

    public void AddBits(string bits)
    {
        var extendedBitArray = new BitArray(_bitArray);
        extendedBitArray.Length += bits.Length;

        for (var i = 0; i < bits.Length; i++)
        {
            var bit = bits[i] == '1';
            extendedBitArray[^(bits.Length - i)] = bit;
        }

        _bitArray = extendedBitArray;
    }

    public static BitsArray Concat(BitsArray leftBits, BitsArray rightBits)
    {
        var left = leftBits._bitArray;
        var right = rightBits._bitArray;
        var bools = new bool[left.Count + right.Count];
        
        left.CopyTo(bools, 0);
        right.CopyTo(bools, left.Count);
        
        return new BitsArray(new BitArray(bools));
    }

    public byte[] ToBytes()
    {
        var bytes = new byte[(int)Math.Ceiling((double)_bitArray.Length / 8)];
        _bitArray.CopyTo(bytes, 0);
        return bytes;
    }

    public override string ToString()
    {
        var bits = new StringBuilder();
        for (var i = 0; i < _bitArray.Count; i++)
            bits.Append(_bitArray[i] ? '1' : '0');
        return bits.ToString();
    }

    public override bool Equals(object? obj)
    {
        var otherBitArray = obj as BitsArray;
        return otherBitArray != null && _bitArray.Equals(otherBitArray._bitArray);
    }

    public override int GetHashCode()
    {
        return _bitArray.GetHashCode();
    }
}