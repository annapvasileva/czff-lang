namespace Compiler.SourceFiles;

public struct Header
{
    public readonly byte[] MagicalNumber = "ball"u8.ToArray();

    public readonly byte[] Version = [0, 0, 1];
    
    public Header()
    {
    }
}