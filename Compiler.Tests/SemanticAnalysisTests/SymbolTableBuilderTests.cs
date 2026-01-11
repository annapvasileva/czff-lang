using System.Text.Json;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis;
using Compiler.SemanticAnalysis.Models;
using Compiler.Tests.Storage;
using Xunit.Abstractions;

namespace Compiler.Tests.SemanticAnalysisTests;

public class SymbolTableBuilderTests
{
    private ITestOutputHelper _output;
    public SymbolTableBuilderTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void OneEmptyFunctionTest()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;"));
        var mainBodyTable = new SymbolTable(expectedTable);

        var ast = AstStore.GetAst("OneEmptyFunction");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var table = symbolTableBuilder.SymbolTable;
        
        var json1 = JsonSerializer.Serialize(expectedTable, 
            new JsonSerializerOptions { WriteIndented = true });
        var json2 = JsonSerializer.Serialize(table, 
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }
    
    [Fact]
    public void OneFunctionWithVariableDeclarationsTest()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 3 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("a", new VariableSymbol("a", "I;", 0));
        mainBodyTable.Symbols.Add("b", new VariableSymbol("b", "I;", 1));
        mainBodyTable.Symbols.Add("res", new VariableSymbol("res", "I;", 2));
        
        var ast = AstStore.GetAst("OneFunctionWithVariableDeclarations");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var table = symbolTableBuilder.SymbolTable;
        
        var json1 = JsonSerializer.Serialize(expectedTable, 
            new JsonSerializerOptions { WriteIndented = true });
        var json2 = JsonSerializer.Serialize(table, 
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void SeveralFunctionsWithVariableDeclarationsTest()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 3 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("a", new VariableSymbol("a", "I;", 0));
        mainBodyTable.Symbols.Add("b", new VariableSymbol("b", "I;", 1));
        mainBodyTable.Symbols.Add("res", new VariableSymbol("res", "I;", 2));

        expectedTable.Symbols.Add("foo", new FunctionSymbol("foo", "void;") { LocalsLength = 2, Index = 1});
        var fooBodyTable = new SymbolTable(expectedTable);
        fooBodyTable.Symbols.Add("a", new VariableSymbol("a", "I;", 0));
        fooBodyTable.Symbols.Add("b", new VariableSymbol("b", "I;", 1));

        var ast = AstStore.GetAst("SeveralFunctionsWithVariableDeclarations");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var table = symbolTableBuilder.SymbolTable;
        
        var json1 = JsonSerializer.Serialize(expectedTable, 
            new JsonSerializerOptions { WriteIndented = true });
        var json2 = JsonSerializer.Serialize(table, 
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void ArrayDeclarationAndIndexingTest()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 3 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("a", new VariableSymbol("a", "I;", 0));
        mainBodyTable.Symbols.Add("b", new VariableSymbol("b", "I;", 1));
        mainBodyTable.Symbols.Add("arr", new VariableSymbol("arr", "[I;", 2));
        
        var ast = AstStore.GetAst("ArrayDeclarationAndIndexing");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var table = symbolTableBuilder.SymbolTable;
        
        var json1 = JsonSerializer.Serialize(expectedTable, 
            new JsonSerializerOptions { WriteIndented = true });
        var json2 = JsonSerializer.Serialize(table, 
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }
    
    [Fact]
    public void SecondExampleTest()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 2 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("n", new VariableSymbol("n", "I;", 0));
        mainBodyTable.Symbols.Add("arr", new VariableSymbol("arr", "[I;", 1));
        
        var ast = AstStore.GetAst("SecondExample");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var table = symbolTableBuilder.SymbolTable;
        
        var json1 = JsonSerializer.Serialize(expectedTable, 
            new JsonSerializerOptions { WriteIndented = true });
        var json2 = JsonSerializer.Serialize(table, 
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }
    
    [Fact]
    public void ThirdExampleTest()
    {
        var expectedTable = new SymbolTable(null);
        var paramA = new VariableSymbol("a", "I;", 0);
        var paramB = new VariableSymbol("b", "I;", 1);
        var sumParams = new List<VariableSymbol>() { paramA, paramB };
        expectedTable.Symbols.Add("Sum", new FunctionSymbol("Sum", "I;") { LocalsLength = 2, Index = 0, Parameters = sumParams });
        var fooBodyTable = new SymbolTable(expectedTable);
        fooBodyTable.Symbols.Add("a", paramA);
        fooBodyTable.Symbols.Add("b", paramB);
        
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 3, Index = 1 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("x", new VariableSymbol("x", "I;", 0));
        mainBodyTable.Symbols.Add("y", new VariableSymbol("y", "I;", 1));
        mainBodyTable.Symbols.Add("z", new VariableSymbol("z", "I;", 2));
        
        var ast = AstStore.GetAst("ThirdExample");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var table = symbolTableBuilder.SymbolTable;
        
        var json1 = JsonSerializer.Serialize(expectedTable, 
            new JsonSerializerOptions { WriteIndented = true });
        var json2 = JsonSerializer.Serialize(table, 
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void FourthExampleTest()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 2 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("x", new VariableSymbol("x", "I;", 0));
        mainBodyTable.Symbols.Add("i", new VariableSymbol("i", "I;", 1));

        var ast = AstStore.GetAst("FourthExample");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var table = symbolTableBuilder.SymbolTable;

        var json1 = JsonSerializer.Serialize(expectedTable, 
            new JsonSerializerOptions { WriteIndented = true });
        var json2 = JsonSerializer.Serialize(table, 
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }
}