using System.Text.Json;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis;
using Compiler.SemanticAnalysis.Models;
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
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void"));
        var mainBodyTable = new SymbolTable(expectedTable);
        
        var ast = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode { },
                    new BlockNode(
                        new List<StatementNode> { })
                )
            }));
        var symbolTableBuilder = new SymbolTableBuilder();
        symbolTableBuilder.Visit((ProgramNode)ast.Root);
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
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void"));
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("a", new VariableSymbol("a", "I", 0));
        mainBodyTable.Symbols.Add("b", new VariableSymbol("b", "I", 1));
        mainBodyTable.Symbols.Add("res", new VariableSymbol("res", "I", 2));
        
        var ast = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode { },
                    new BlockNode(
                        new List<StatementNode>
                        {
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "a",
                                new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "b",
                                new LiteralExpressionNode("3", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "res",
                                new BinaryExpressionNode(
                                    new IdentifierExpressionNode("a"),
                                    new IdentifierExpressionNode("b"),
                                    BinaryOperatorType.Addition))
                        })
                )
            }));
        var symbolTableBuilder = new SymbolTableBuilder();
        symbolTableBuilder.Visit((ProgramNode)ast.Root);
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
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void"));
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("a", new VariableSymbol("a", "I", 0));
        mainBodyTable.Symbols.Add("b", new VariableSymbol("b", "I", 1));
        mainBodyTable.Symbols.Add("res", new VariableSymbol("res", "I", 2));

        expectedTable.Symbols.Add("foo", new FunctionSymbol("foo", "void"));
        var fooBodyTable = new SymbolTable(expectedTable);
        fooBodyTable.Symbols.Add("a", new VariableSymbol("a", "I", 0));
        fooBodyTable.Symbols.Add("b", new VariableSymbol("b", "I", 1));
        
        var ast = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode { },
                    new BlockNode(
                        new List<StatementNode>
                        {
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "a",
                                new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "b",
                                new LiteralExpressionNode("3", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "res",
                                new BinaryExpressionNode(
                                    new IdentifierExpressionNode("a"),
                                    new IdentifierExpressionNode("b"),
                                    BinaryOperatorType.Addition))
                        })
                ),
                new (
                    new SimpleTypeNode("void"),
                    "foo",
                    new FunctionParametersNode { },
                    new BlockNode(
                        new List<StatementNode>
                        {
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "a",
                                new LiteralExpressionNode("6", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "b",
                                new LiteralExpressionNode("5", LiteralType.IntegerLiteral))
                        }
                    )
                )
            }));
        var symbolTableBuilder = new SymbolTableBuilder();
        symbolTableBuilder.Visit((ProgramNode)ast.Root);
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