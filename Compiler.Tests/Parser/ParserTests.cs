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
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode(){ },
                    new BlockNode(new List<StatementNode>(){ })
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
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode(){ },
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
                                "c")
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
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode(){ },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new PrintStatementNode(new LiteralExpressionNode("123", LiteralType.IntegerLiteral)),
                            new PrintStatementNode(new IdentifierExpressionNode("res"))
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
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode(){ },
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
                                    BinaryOperatorType.Addition))
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
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                   "Main",
                    new FunctionParametersNode(){ },
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
                            new PrintStatementNode(new IdentifierExpressionNode("res"))
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
                        }
                        
                        func void boo() {
                            print 1234567;
                        }
                        """;
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                   "Foo",
                    new FunctionParametersNode(){ },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new PrintStatementNode(new LiteralExpressionNode("123", LiteralType.IntegerLiteral)),
                        })
                ),
                new (
                    new SimpleTypeNode("void"),
                    "boo",
                    new FunctionParametersNode(){ },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new PrintStatementNode(new LiteralExpressionNode("1234567", LiteralType.IntegerLiteral)),
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
}