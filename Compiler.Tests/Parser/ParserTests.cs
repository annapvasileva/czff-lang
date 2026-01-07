using System.Text.Json;
using Compiler.Lexer;
using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Xunit.Abstractions;

namespace Compiler.Tests.Parser;

public class ParserTests
{
    private ITestOutputHelper _output;

    public ParserTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void EmptyMainTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            return;
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode() { },
                    new BlockNode(new List<StatementNode>()
                    {
                        new ReturnStatementNode(null)
                    })
                )
            }));

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);

        var ast = parser.Parse();

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void VariableDeclarationTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            var int a = 2;
                            var int b = a;
                            var int c;
                            
                            return;
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
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
                                new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "b",
                                new IdentifierExpressionNode("a")),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "c"),
                            new ReturnStatementNode(null)
                        })
                )
            }));

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);

        var ast = parser.Parse();

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(json1, json2);
    }
    
    [Fact]
    public void IdentifiersNamingTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            var int a1 = 2;
                            var int b_two2 = a;
                            var int _c;
                            
                            return;
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
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
                                "a1",
                                new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "b_two2",
                                new IdentifierExpressionNode("a")),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "_c"),
                            new ReturnStatementNode(null)
                        })
                )
            }));

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);

        var ast = parser.Parse();

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void PrintStatementTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            print 123;
                            print res;
                            return;
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new(
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode() { },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new PrintStatementNode(new LiteralExpressionNode("123", LiteralType.IntegerLiteral)),
                            new PrintStatementNode(new IdentifierExpressionNode("res")),
                            new ReturnStatementNode(null)
                        })
                )
            }));

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);

        var ast = parser.Parse();

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void ParseExpressionTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            var int a = 1 + (2 + 3);
                            var int b = (1 + 2) + 3;
                            
                            return;
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
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
                                new BinaryExpressionNode(
                                    new LiteralExpressionNode("1", LiteralType.IntegerLiteral),
                                    new BinaryExpressionNode(
                                        new LiteralExpressionNode("2", LiteralType.IntegerLiteral),
                                        new LiteralExpressionNode("3", LiteralType.IntegerLiteral),
                                        BinaryOperatorType.Addition),
                                    BinaryOperatorType.Addition)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "b",
                                new BinaryExpressionNode(
                                    new BinaryExpressionNode(
                                        new LiteralExpressionNode("1", LiteralType.IntegerLiteral),
                                        new LiteralExpressionNode("2", LiteralType.IntegerLiteral),
                                        BinaryOperatorType.Addition),
                                    new LiteralExpressionNode("3", LiteralType.IntegerLiteral),
                                    BinaryOperatorType.Addition)),
                            new ReturnStatementNode(null)
                        })
                )
            }));

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);

        var ast = parser.Parse();

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void BaseExampleTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            var int a = 2;
                            var int b = 3;
                            var int res = a + b;
                            print res;
                            
                            return;
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
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
                                    BinaryOperatorType.Addition)),
                            new PrintStatementNode(new IdentifierExpressionNode("res")),
                            new ReturnStatementNode(null)
                        })
                )
            }));

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);

        var ast = parser.Parse();

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void SeveralFunctionsExampleTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Foo() {
                            print 123;
                            return;
                        }

                        func void boo() {
                            print 1234567;
                            return;
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new(
                    new SimpleTypeNode("void"),
                    "Foo",
                    new FunctionParametersNode() { },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new PrintStatementNode(new LiteralExpressionNode("123", LiteralType.IntegerLiteral)),
                            new ReturnStatementNode(null)
                        })
                ),
                new(
                    new SimpleTypeNode("void"),
                    "boo",
                    new FunctionParametersNode() { },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new PrintStatementNode(new LiteralExpressionNode("1234567", LiteralType.IntegerLiteral)),
                            new ReturnStatementNode(null)
                        })
                )
            }));

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);

        var ast = parser.Parse();

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(json1, json2);
    }

    [Fact]
    public void ArrayDeclarationTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            var array<int> arr = new(5)[];
                            
                            return;
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new(
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode() { },
                    new BlockNode(new List<StatementNode>()
                    {
                        new VariableDeclarationNode(
                            new ArrayTypeNode(new SimpleTypeNode("int")), 
                            "arr",
                            new ArrayCreationExpressionNode(new LiteralExpressionNode("5", LiteralType.IntegerLiteral))),
                        new ReturnStatementNode(null)
                    })
                )
            }));

        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);

        var ast = parser.Parse();

        var json1 = JsonSerializer.Serialize(expectedAst,
            new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(json1);

        var json2 = JsonSerializer.Serialize(ast,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(json1, json2);
    }

// public void ArrayDeclarationTest()
    // {
    //     var expectedAst = new AstTree(new ProgramNode(
    //         new List<FunctionDeclarationNode>()
    //         {
    //             new (
    //                 new SimpleTypeNode("void"),
    //                 "Foo",
    //                 new FunctionParametersNode(){ },
    //                 new BlockNode(
    //                     new List<StatementNode>()
    //                     {
    //                         new VariableDeclarationNode(
    //                             new SimpleTypeNode("int"),
    //                             "n",
    //                             new LiteralExpressionNode("5", LiteralType.IntegerLiteral)),
    //                         new VariableDeclarationNode(
    //                             new ArrayTypeNode(
    //                                 new SimpleTypeNode("int")),
    //                                 "arr",
    //                                 new ArrayCreationExpressionNode(new IdentifierExpressionNode("n"))),
    //                         new AssignmentStatementNode(
    //                             new ArrayIndexExpressionNode(
    //                                 new IdentifierExpressionNode("arr"),
    //                                 new LiteralExpressionNode("0", LiteralType.IntegerLiteral)), new LiteralExpressionNode("-1", LiteralType.IntegerLiteral)),
    //                     })
    //             )
    //         }));
    // }
}

/*
func void Main() {
    var int n = 5;
    var array<int> arr = new(n)[];
    arr[0] = -1;
    arr[1] = 2;
    arr[2] = arr[0] + arr[1];
    arr[3] = -(arr[0] * arr[1]);
    arr[4] = arr[0] * (arr[1] + arr[2]) + arr[3];
    
    print arr[0];
    print arr[1];
    print arr[2];
    print arr[3];
    print arr[4];

    return;
}
*/