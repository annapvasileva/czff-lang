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

public class DeadCodeEliminationTests
{
    private ITestOutputHelper _output;

    public DeadCodeEliminationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ReturnTest()
    {
        string source = """
                        func void Main() {
                            print 1;
                            print 2;
                            print 3;
                            return;

                            var int a = 10;
                            print a;
                            return;
                        }
                        """;
        
        var expectedAst = GetReturnTestTree();
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();

        var symbolTableBuilder = new SymbolTableBuilder();
        var pipelineUnits = new List<INodeVisitor>()
        {
            symbolTableBuilder,
            new SemanticAnalyzer(symbolTableBuilder.SymbolTable),
            new DeadCodeEliminationOptimizer(symbolTableBuilder.SymbolTable),
            new SymbolTableBuilder(),
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
    public void NestedReturnTest()
    {
        string source = """
                        func void Main() {
                            var int a = 1;
                            if (a < 10) {
                                print 123;
                                return;
                                print 1;
                                print 2;
                                print 3;
                                if (1 > 0) {
                                    print 4;
                                }
                            }

                            print 1;
                            return;
                        }
                        """;
        var expectedAst = GetNestedReturnTestTree();
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();

        var symbolTableBuilder = new SymbolTableBuilder();
        var pipelineUnits = new List<INodeVisitor>()
        {
            symbolTableBuilder,
            new SemanticAnalyzer(symbolTableBuilder.SymbolTable),
            new DeadCodeEliminationOptimizer(symbolTableBuilder.SymbolTable),
            new SymbolTableBuilder(),
        };
        Pipeline.Run(ast, pipelineUnits);
        
        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void LoopsBreakTest()
    {
        string source = """
                        func void Main() {
                            for (var int i = 0; i < 10; i = i + 1) {
                                for (var int j = 0; j < 10; j = j + 1) {
                                    if (j > i) {
                                        break;
                                        var int c = 10;
                                        print c;
                                    }
                                }
                                print i;
                            }

                            print 1;
                            return;
                        }
                        """;
        

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();

        var symbolTableBuilder = new SymbolTableBuilder();
        var pipelineUnits = new List<INodeVisitor>()
        {
            symbolTableBuilder,
            new SemanticAnalyzer(symbolTableBuilder.SymbolTable),
            new DeadCodeEliminationOptimizer(symbolTableBuilder.SymbolTable),
            new SymbolTableBuilder(),
        };
        Pipeline.Run(ast, pipelineUnits);
        
        var expectedAst = GetLoopsBreakTestTree();
        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(json1, json2);
    }
    
    [Fact]
    public void UnusedVariablesTest()
    {
        string source = """
                        func void Main() {
                            var bool flag2 = false;
                            var int x = 9;
                            for (var int i = 0; i < 10; i = i + 1) {
                                for (var int j = 0; j < 10; j = j + 1) {
                                    if (j > i) {
                                        flag2 = true;
                                        break;
                                        var int c = 10;
                                        print c;
                                    }
                                }
                                print i;
                            }
                            var int c = 9;
                            print 1;
                            print flag2;
                            return;
                        }
                        """;
        

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();

        var symbolTableBuilder = new SymbolTableBuilder();
        var pipelineUnits = new List<INodeVisitor>()
        {
            symbolTableBuilder,
            new SemanticAnalyzer(symbolTableBuilder.SymbolTable),
            new DeadCodeEliminationOptimizer(symbolTableBuilder.SymbolTable),
            new SymbolTableBuilder(),
        };
        Pipeline.Run(ast, pipelineUnits);
        
        var expectedAst = GetUnusedVariablesTestTree();
        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void UnusedFunctionsTest()
    {
        string source = """
                        func void Unused() {
                            return;
                        }
                        
                        func void f() {
                            return;
                        }
                        
                        func void A() {
                            B();
                            return;
                        }
                        
                        func void B() {
                            return;
                        }
                        
                        func void Main() {
                            f();
                            return;
                        }
                        """;
        

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();

        var symbolTableBuilder = new SymbolTableBuilder();
        var pipelineUnits = new List<INodeVisitor>()
        {
            symbolTableBuilder,
            new SemanticAnalyzer(symbolTableBuilder.SymbolTable),
            new DeadCodeEliminationOptimizer(symbolTableBuilder.SymbolTable),
            new SymbolTableBuilder(),
        };
        Pipeline.Run(ast, pipelineUnits);
        
        var expectedAst = GetUnusedFunctionsTestTree();
        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json2);

        Assert.Equal(json1, json2);
    }

    // test data
    private AstTree GetReturnTestTree()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 0 });
        var mainBodyTable = new SymbolTable(expectedTable);

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
                            new PrintStatementNode(new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                            new PrintStatementNode(new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                            new PrintStatementNode(new LiteralExpressionNode("3", LiteralType.IntegerLiteral)),
                            new ReturnStatementNode(null)
                        })
                    {
                        Scope = mainBodyTable,
                    }
                )
            }));
    }

    private AstTree GetNestedReturnTestTree()
    {
        
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 1 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("a", new VariableSymbol("a", "I;", 0));
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
                                new SimpleTypeNode("int"),
                                "a",
                                new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                            new IfStatementNode(
                                new BinaryExpressionNode(
                                    new IdentifierExpressionNode("a"),
                                    new LiteralExpressionNode("10",  LiteralType.IntegerLiteral),
                                    BinaryOperatorType.Less),
                                new BlockNode(new  List<StatementNode>()
                                {
                                    new PrintStatementNode(new LiteralExpressionNode("123", LiteralType.IntegerLiteral)),
                                    new ReturnStatementNode(null),
                                })
                                {
                                    Scope = ifBodyTable,
                                },
                                new List<ElifStatementNode>(),
                                null),
                            new  PrintStatementNode(new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                            new ReturnStatementNode(null),
                        })
                    {
                        Scope = mainBodyTable,
                    }
                )
            }));
    }

    private AstTree GetLoopsBreakTestTree()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 2 });
        var mainBodyTable = new SymbolTable(expectedTable);
        var firstForBodyTable = new SymbolTable(mainBodyTable);
        firstForBodyTable.Symbols.Add("i", new VariableSymbol("i", "I;", 0));
        var secondForBodyTable = new SymbolTable(firstForBodyTable);
        secondForBodyTable.Symbols.Add("j", new VariableSymbol("j", "I;", 1));
        var ifBodyTable = new SymbolTable(secondForBodyTable);

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
                            new ForStatementNode(
                                new VariableDeclarationNode(
                                    new SimpleTypeNode("int"),
                                    "i",
                                    new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
                                new BinaryExpressionNode(
                                    new IdentifierExpressionNode("i"),
                                    new LiteralExpressionNode("10", LiteralType.IntegerLiteral),
                                    BinaryOperatorType.Less),
                                new IdentifierAssignmentStatementNode(
                                    new IdentifierExpressionNode("i"),
                                    new BinaryExpressionNode(
                                        new IdentifierExpressionNode("i"),
                                        new LiteralExpressionNode("1", LiteralType.IntegerLiteral),
                                        BinaryOperatorType.Addition)),
                                new BlockNode(new  List<StatementNode>()
                                {
                                    new ForStatementNode(
                                        new VariableDeclarationNode(
                                            new SimpleTypeNode("int"),
                                            "j",
                                            new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
                                        new BinaryExpressionNode(
                                            new IdentifierExpressionNode("j"),
                                            new LiteralExpressionNode("10", LiteralType.IntegerLiteral),
                                            BinaryOperatorType.Less),
                                        new IdentifierAssignmentStatementNode(
                                            new IdentifierExpressionNode("j"),
                                            new BinaryExpressionNode(
                                                new IdentifierExpressionNode("j"),
                                                new LiteralExpressionNode("1", LiteralType.IntegerLiteral),
                                                BinaryOperatorType.Addition)),
                                        new BlockNode(new  List<StatementNode>()
                                        {
                                            new IfStatementNode(
                                                new BinaryExpressionNode(
                                                    new IdentifierExpressionNode("j"),
                                                    new IdentifierExpressionNode("i"),
                                                    BinaryOperatorType.Greater),
                                                new BlockNode(new  List<StatementNode>()
                                                {
                                                    new BreakStatementNode(),
                                                })
                                                {
                                                    Scope = ifBodyTable,
                                                },
                                                new List<ElifStatementNode>(),
                                                null)
                                        })
                                        {
                                            Scope = secondForBodyTable,
                                        }),
                                    new PrintStatementNode(new IdentifierExpressionNode("i")),
                                })
                                {
                                    Scope = firstForBodyTable,
                                }),
                            new PrintStatementNode(new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                            new ReturnStatementNode(null),
                        })
                    {
                        Scope = mainBodyTable,
                    }
                )
            }));
    }
    
    private AstTree GetUnusedVariablesTestTree()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 3 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("flag2", new VariableSymbol("flag2", "B;", 0));
        var firstForBodyTable = new SymbolTable(mainBodyTable);
        firstForBodyTable.Symbols.Add("i", new VariableSymbol("i", "I;", 1));
        var secondForBodyTable = new SymbolTable(firstForBodyTable);
        secondForBodyTable.Symbols.Add("j", new VariableSymbol("j", "I;", 2));
        var ifBodyTable = new SymbolTable(secondForBodyTable);

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
                                "flag2",
                                new LiteralExpressionNode("false", LiteralType.BooleanLiteral)),
                            new ForStatementNode(
                                new VariableDeclarationNode(
                                    new SimpleTypeNode("int"),
                                    "i",
                                    new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
                                new BinaryExpressionNode(
                                    new IdentifierExpressionNode("i"),
                                    new LiteralExpressionNode("10", LiteralType.IntegerLiteral),
                                    BinaryOperatorType.Less),
                                new IdentifierAssignmentStatementNode(
                                    new IdentifierExpressionNode("i"),
                                    new BinaryExpressionNode(
                                        new IdentifierExpressionNode("i"),
                                        new LiteralExpressionNode("1", LiteralType.IntegerLiteral),
                                        BinaryOperatorType.Addition)),
                                new BlockNode(new  List<StatementNode>()
                                {
                                    new ForStatementNode(
                                        new VariableDeclarationNode(
                                            new SimpleTypeNode("int"),
                                            "j",
                                            new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
                                        new BinaryExpressionNode(
                                            new IdentifierExpressionNode("j"),
                                            new LiteralExpressionNode("10", LiteralType.IntegerLiteral),
                                            BinaryOperatorType.Less),
                                        new IdentifierAssignmentStatementNode(
                                            new IdentifierExpressionNode("j"),
                                            new BinaryExpressionNode(
                                                new IdentifierExpressionNode("j"),
                                                new LiteralExpressionNode("1", LiteralType.IntegerLiteral),
                                                BinaryOperatorType.Addition)),
                                        new BlockNode(new  List<StatementNode>()
                                        {
                                            new IfStatementNode(
                                                new BinaryExpressionNode(
                                                    new IdentifierExpressionNode("j"),
                                                    new IdentifierExpressionNode("i"),
                                                    BinaryOperatorType.Greater),
                                                new BlockNode(new  List<StatementNode>()
                                                {
                                                    new IdentifierAssignmentStatementNode(
                                                        new IdentifierExpressionNode("flag2"),
                                                        new LiteralExpressionNode("true", LiteralType.BooleanLiteral)),
                                                    new BreakStatementNode(),
                                                })
                                                {
                                                    Scope = ifBodyTable,
                                                },
                                                new List<ElifStatementNode>(),
                                                null)
                                        })
                                        {
                                            Scope = secondForBodyTable,
                                        }),
                                    new PrintStatementNode(new IdentifierExpressionNode("i")),
                                })
                                {
                                    Scope = firstForBodyTable,
                                }),
                            new PrintStatementNode(new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                            new PrintStatementNode(new IdentifierExpressionNode("flag2")),
                            new ReturnStatementNode(null),
                        })
                    {
                        Scope = mainBodyTable,
                    }
                )
            }));
    }

    public AstTree GetUnusedFunctionsTestTree()
    {
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 0 });
        expectedTable.Symbols.Add("f", new FunctionSymbol("f", "void;") { LocalsLength = 0 });
        var mainBodyTable = new SymbolTable(expectedTable);
        var fBodyTable = new SymbolTable(expectedTable);

        return new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new(
                    new SimpleTypeNode("void"),
                    "f",
                    new FunctionParametersNode() { },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new ReturnStatementNode(null)
                        })
                    {
                        Scope = fBodyTable,
                    }
                ),
                new(
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode() { },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new ExpressionStatementNode(new FunctionCallExpressionNode("f", new List<ExpressionNode>())),
                            new ReturnStatementNode(null)
                        })
                    {
                        Scope = mainBodyTable,
                    }
                )
            }));
    }
}