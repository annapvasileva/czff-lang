namespace Compiler.SourceFiles;

public class ConstantItem(byte tag, byte[] data)
{
    public byte Tag { get; set; } = tag;

    public byte[] Data { get; set; } = data;
}