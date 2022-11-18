namespace Archiver5E2D.CompressionAlgorithms;

public class RlePrefix
{
    public byte[] Compress(byte prefix, byte[] content)
    {
        byte? prev = null;
        byte length = 1;
        var result = new List<byte>();
        foreach (byte b in content)
        {
            if (length == 255 || (b != prev && prev != null))
            {
                if (prev == prefix && length == 1)
                {
                    result.Add(prefix);
                    result.Add(0);
                }
                else if ((length >= 4 && prev != prefix) || (length >= 2 && prev == prefix))
                {
                    result.Add(prefix);
                    result.Add((byte)(length - 1));
                    if (prev != null) result.Add(prev.Value);
                }
                else
                {
                    if (prev != null) result.AddRange(Enumerable.Repeat(prev.Value, length));
                }

                length = 1;
            }

            if (b == prev)
            {
                length++;
            }

            prev = b;
        }

        if (prev == prefix && length == 1)
        {
            result.Add(prefix);
            result.Add(0);
        }
        else if ((length >= 4 && prev != prefix) || (length >= 2 && prev == prefix))
        {
            result.Add(prefix);
            result.Add((byte)(length - 1));
            if (prev != null) result.Add(prev.Value);
        }
        else
        {
            if (prev != null) result.Add(prev.Value);
        }

        return result.ToArray();
    }
}