using Compiler.Util;

namespace Compiler.SourceFiles;

public class ConstantItem
{
    public byte Tag { get; set; }

    public byte[] Data { get; set; }

    public ConstantItem(byte tag, byte[] data)
    {
        Tag = tag;
        Data = data;
    }
    
    public ConstantItem(byte tag, string line)
    {
        Tag = tag;
        if (tag != 5)
        {
            throw new ArgumentException("Invalid tag");
        }
        
        int size = line.Length;
        Data = new byte[2 + size];

        byte[] bytes = ByteConverter.IntToU2(size);
        Data[0] =  bytes[0];
        Data[1] =  bytes[1];
        for (int i = 0; i < size; i++)
        {
            Data[i + 2] = (byte)line[i];
        }
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not ConstantItem other)
            return false;

        if (Tag != other.Tag)
            return false;

        if (Data.Length != other.Data.Length)
            return false;

        return Data.AsSpan().SequenceEqual(other.Data);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Tag);

        foreach (byte b in Data)
            hash.Add(b);

        return hash.ToHashCode();
    }
}