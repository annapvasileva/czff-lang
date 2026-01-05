using System.Text.Json.Serialization;

namespace Compiler.SemanticAnalysis.Models;

[JsonDerivedType(typeof(VariableSymbol), typeDiscriminator: "variable")]
[JsonDerivedType(typeof(FunctionSymbol), typeDiscriminator: "function")]
public abstract class Symbol
{
    public string Name { get; }
    public SymbolKind Kind { get; }

    protected Symbol(string name, SymbolKind kind)
    {
        Name = name;
        Kind = kind;
    }
}