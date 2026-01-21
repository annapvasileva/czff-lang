namespace Compiler.SemanticAnalysis.Models;

public class ArraySymbol : Symbol
{
    public string ElementType { get; }

    public int Length { get; set; }

    public ArraySymbol(string name, string type, int length) 
        : base(name, SymbolKind.Array)
    {
        ElementType = type;
        Length = length;
    }
}