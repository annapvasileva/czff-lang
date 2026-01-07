using Compiler.Util;

namespace Compiler.SourceFiles;

public abstract class ConstantItem
{
    public byte Tag { get; set; }

    public byte[] Data { get; set; }

    public ConstantItem(byte tag, byte[] data)
    {
        Tag = tag;
        Data = data;
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