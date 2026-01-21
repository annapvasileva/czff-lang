namespace Compiler.Util;

public class CompilerSettings
{
    public byte[] Version { get; set; } = [0, 0, 0];

    public bool ConstantFolding { get; set; } = false;
    
    public bool DeadCodeElimination { get; set; } = false;
}
