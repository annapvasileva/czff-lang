namespace Compiler.SourceFiles;

public struct Header(byte[] version, byte flags)
{
    public readonly byte[] MagicalNumber { get; }= "ball"u8.ToArray();

    public readonly byte[] Version { get; } = version;

    public byte Flags { get; set; } = flags;
}