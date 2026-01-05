namespace Compiler.SemanticAnalysis.Models;

public class VariableSymbol : Symbol
{
    public string Type { get; } // хранить тип как объект?
    public int Index { get; set; }

    public VariableSymbol(string name, string type, int index) 
        : base(name, SymbolKind.Variable)
    {
        Type = type;
        Index = index;
    }
}