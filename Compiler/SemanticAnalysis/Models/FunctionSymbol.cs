namespace Compiler.SemanticAnalysis.Models;

public class FunctionSymbol : Symbol
{
    public string ReturnType { get; }

    public List<VariableSymbol> Parameters { get; }
    
    public int LocalsLength { get; set; } = 0;

    public int Index { get; set; } = 0;

    public FunctionSymbol(string name, string returnType) 
        : base(name, SymbolKind.Function)
    {
        ReturnType = returnType;
    }
}