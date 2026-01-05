namespace Compiler.SemanticAnalysis.Models;

public class FunctionSymbol : Symbol
{
    public string ReturnType { get; }
    // add parameters in future
    
    public FunctionSymbol(string name, string returnType) 
        : base(name, SymbolKind.Function)
    {
        ReturnType = returnType;
    }
}