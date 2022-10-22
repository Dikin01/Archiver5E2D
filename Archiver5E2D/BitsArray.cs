using BitArray = System.Collections.BitArray;
using StringBuilder = System.Text.StringBuilder;

namespace Archiver5E2D;

public class BitsArray
{
    public BitArray bitArray { get; protected set; }

    public BitsArray()
    {
        bitArray = new BitArray(0);
    }

    public BitsArray(BitArray bitArray)
    {
        this.bitArray = bitArray;
    }

    public BitsArray(BitsArray bitsArray)
    {
        this.bitArray = new BitArray(bitsArray.bitArray);
    }

    public BitsArray(string bits)
    {
        this.bitArray = new BitArray(0);
        this.AddBits(bits);
    }

    public void AddBit(bool bit)
    {
        var extendedBitArray = new BitArray(this.bitArray);
        extendedBitArray.Length += 1;
        extendedBitArray[extendedBitArray.Length - 1] = bit;
        this.bitArray = extendedBitArray;
    }

    public void AddBits(string bits)
    {
        var extendedBitArray = new BitArray(this.bitArray);
        extendedBitArray.Length += bits.Length;
        for (int i = 0; i < bits.Length; i++)
        {
            bool bit = false;
            if (bits[i] == '1')
            {
                bit = true;
            }
            extendedBitArray[extendedBitArray.Length - (bits.Length - i)] = bit;
        }
        this.bitArray = extendedBitArray;
    }

    public static BitsArray Concat(BitsArray leftBits, BitsArray rightBits)
    {
        var left = leftBits.bitArray;
        var right = rightBits.bitArray;
        var bools = new bool[left.Count + right.Count];
        left.CopyTo(bools, 0);
        right.CopyTo(bools, left.Count);
        return new BitsArray(new BitArray(bools));
    }

    public int Length()
    {
        return bitArray.Length;
    }

    public byte[] ToBytes()
    {
        byte[] bytes = new byte[Convert.ToInt32(Math.Ceiling((double)this.bitArray.Length / 8))];
        this.bitArray.CopyTo(bytes, 0);
        return bytes;
    }

    public override string ToString()
    {
        var bits = new StringBuilder();
        for (int i = 0; i < bitArray.Count; i++)
        {
            bits.Append(bitArray[i] ? '1' : '0');
        }
        return bits.ToString();
    }

    public override bool Equals(object? obj)
    {
        return (obj != null && this.GetType().Equals(obj.GetType())) && this.bitArray.Equals(((BitsArray)obj).bitArray);
    }

    public override int GetHashCode()
    {
        return this.bitArray.GetHashCode();
    }
}