using System.Text.Json;
using Compiler.CompilerPipeline;
using Compiler.Lexer;
using Compiler.Optimizations;
using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis;
using Compiler.SemanticAnalysis.Models;
using Xunit.Abstractions;

namespace Compiler.Tests.OptimizerTests;

public class ConstantFoldingTests
{
    private ITestOutputHelper _output;

    public ConstantFoldingTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void ArithmeticOperationsWithIntConstantsTest()
    {
        string source = """
                        func void Main() {
                            var int a = -(1 + 2 * 3 / 4 - 10);
                            return;
                        }
                        """;

        var expectedAst = GetArithmeticOperationsWithIntConstantsTestData();
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();

        var symbolTableBuilder = new SymbolTableBuilder();
        var pipelineUnits = new List<INodeVisitor>()
        {
            symbolTableBuilder,
            new SemanticAnalyzer(symbolTableBuilder.SymbolTable),
            new ConstantFoldingOptimizer(),
        };
        Pipeline.Run(ast, pipelineUnits);

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void BoolOperationsTest()
    {
        string source = """
                        func void Main() {
                            var bool flag = false;
                            flag = !((1 > 3) && (5 == 5) || false && (2 <= 3) || (5 != 5));
                            return;
                        }
                        """;

        var expectedAst = GetBoolOperationsTestData();
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();

        var symbolTableBuilder = new SymbolTableBuilder();
        var pipelineUnits = new List<INodeVisitor>()
        {
            symbolTableBuilder,
            new SemanticAnalyzer(symbolTableBuilder.SymbolTable),
            new ConstantFoldingOptimizer(),
        };
        Pipeline.Run(ast, pipelineUnits);

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }
    
    [Fact]
    public void Int64OperationTest()
    {
        string source = """
                        func void Main() {
                            var int64 a = 60L * 60L;
                            var int64 b = 2L * a + 1L;
                            if (10L % 2L == 0L) {
                                print 0;
                            }
                            a = a / 20L;
                            return;
                        }
                        """;

        var expectedAst = GetInt64OperationTestData();
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();

        var symbolTableBuilder = new SymbolTableBuilder();
        var pipelineUnits = new List<INodeVisitor>()
        {
            symbolTableBuilder,
            new SemanticAnalyzer(symbolTableBuilder.SymbolTable),
            new ConstantFoldingOptimizer(),
        };
        Pipeline.Run(ast, pipelineUnits);

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }
    
    private AstTree GetArithmeticOperationsWithIntConstantsTestData()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 1 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("a", new VariableSymbol("a", "I;", 0));

        return new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new(
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode() { },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "a",
                                new LiteralExpressionNode("8", LiteralType.IntegerLiteral)),
                            new ReturnStatementNode(null)
                        })
                    {
                        Scope = mainBodyTable,
                    }
                )
            }));
    }
    
    private AstTree GetBoolOperationsTestData()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 0 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("flag", new VariableSymbol("flag", "B;", 0));

        return new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new(
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode() { },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new VariableDeclarationNode(
                                new SimpleTypeNode("bool"),
                                "flag",
                                new LiteralExpressionNode("false", LiteralType.BooleanLiteral)),
                            new IdentifierAssignmentStatementNode(
                                new IdentifierExpressionNode("flag"),
                                new LiteralExpressionNode("true", LiteralType.BooleanLiteral)),
                            new ReturnStatementNode(null)
                        })
                    {
                        Scope = mainBodyTable,
                    }
                )
            }));
    }

    private AstTree GetInt64OperationTestData()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 2 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("a", new VariableSymbol("a", "I8;", 0));
        mainBodyTable.Symbols.Add("b", new VariableSymbol("b", "I8;", 1));
        var ifBodyTable = new SymbolTable(mainBodyTable);

        return new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new(
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode() { },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int64"),
                                "a",
                                new LiteralExpressionNode("3600", LiteralType.Integer64Literal)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int64"),
                                "b",
                                new BinaryExpressionNode(
                                    new BinaryExpressionNode(
                                        new LiteralExpressionNode("2", LiteralType.Integer64Literal),
                                        new IdentifierExpressionNode("a"),
                                        BinaryOperatorType.Multiplication),
                                    new LiteralExpressionNode(
                                        "1",
                                        LiteralType.Integer64Literal),
                                    BinaryOperatorType.Addition)),
                            new IfStatementNode(
                                new LiteralExpressionNode("true", LiteralType.BooleanLiteral),
                                new BlockNode(new  List<StatementNode>()
                                {
                                    new PrintStatementNode(new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
                                }) { Scope = ifBodyTable },
                                new List<ElifStatementNode>(),
                                null),
                            new IdentifierAssignmentStatementNode(
                                new IdentifierExpressionNode("a"),
                                new BinaryExpressionNode(
                                    new IdentifierExpressionNode("a"),
                                    new LiteralExpressionNode("20", LiteralType.Integer64Literal),
                                    BinaryOperatorType.Division)),
                            new ReturnStatementNode(null)
                        })
                    {
                        Scope = mainBodyTable,
                    }
                )
            }));
    }
}