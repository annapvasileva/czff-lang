using System.Text.Json.Serialization;

namespace Compiler.SemanticAnalysis.Models;

public class SymbolTable
{
    [JsonPropertyName("symbols")]
    public Dictionary<string, Symbol> Symbols { get; private set; } = new();

    [JsonPropertyName("children")]
    public List<SymbolTable> Children { get; private set; } = new();

    public int LocalCount => Symbols.Count;
    
    [JsonIgnore]
    public SymbolTable? Parent { get; }

    public SymbolTable(SymbolTable? parent)
    {
        Parent = parent;
        if (parent != null)
        {
            parent.Children.Add(this);
        }
    }

    public bool Declare(Symbol symbol)
    {
        if (Symbols.ContainsKey(symbol.Name))
            return false;

        Symbols[symbol.Name] = symbol;
        return true;
    }

    public Symbol Lookup(string name)
    {
        if (Symbols.TryGetValue(name, out var symbol))
            return symbol;

        if (Parent == null)
            throw new Exception($"Symbol {name} could not be found.");
        
        return Parent.Lookup(name);
    }
}