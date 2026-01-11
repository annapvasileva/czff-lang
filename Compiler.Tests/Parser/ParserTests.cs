using System.Text.Json;
using Compiler.Lexer;
using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.Tests.Storage;
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
    public void MultiplicationUnaryExpressionParseExpressionTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            var int c = 5;
                            var int a = 1 * (2 + 3);
                            var int b = (a + 2) * (-c) + 10;
                            
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
                                "c",
                                new LiteralExpressionNode("5",  LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "a",
                                new BinaryExpressionNode(
                                    new LiteralExpressionNode(
                                        "1",
                                        LiteralType.IntegerLiteral),
                                    new BinaryExpressionNode(
                                        new LiteralExpressionNode("2", LiteralType.IntegerLiteral),
                                        new  LiteralExpressionNode("3", LiteralType.IntegerLiteral),
                                        BinaryOperatorType.Addition),
                                    BinaryOperatorType.Multiplication)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "b",
                                new BinaryExpressionNode(
                                    new BinaryExpressionNode(
                                        new BinaryExpressionNode(
                                            new IdentifierExpressionNode("a"),
                                            new LiteralExpressionNode("2",  LiteralType.IntegerLiteral),
                                            BinaryOperatorType.Addition),
                                        new UnaryExpressionNode(
                                            UnaryOperatorType.Minus,
                                            new IdentifierExpressionNode("c")),
                                        BinaryOperatorType.Multiplication),
                                    new LiteralExpressionNode("10",  LiteralType.IntegerLiteral),
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
    public void VariableAssigmentTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            var int n;
                            n = 0;
                            
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
                            new SimpleTypeNode("int"),
                            "n"),
                        new IdentifierAssignmentStatementNode(
                            new IdentifierExpressionNode("n"),
                            new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
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
                            var array<int> arr = new int(5)[];
                            
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
                            new ArrayCreationExpressionNode(
                                new SimpleTypeNode("int"),
                                new LiteralExpressionNode("5", LiteralType.IntegerLiteral))),
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
    public void IndexArrayTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            var int idx = 2;
                            var array<int> arr = new int(3)[];
                            arr[0] = 1;
                            arr[1] = -arr[0];
                            arr[idx] = (-arr[0] + arr[1]) * arr[0];
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
                            new SimpleTypeNode("int"),
                            "idx",
                            new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                        new VariableDeclarationNode(
                            new ArrayTypeNode(new SimpleTypeNode("int")), 
                            "arr",
                            new ArrayCreationExpressionNode(
                                new SimpleTypeNode("int"),
                                new LiteralExpressionNode("3", LiteralType.IntegerLiteral))),
                        new ArrayAssignmentStatementNode(
                            new ArrayIndexExpressionNode(
                                new IdentifierExpressionNode("arr"),
                                new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
                            new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                        new ArrayAssignmentStatementNode(
                            new ArrayIndexExpressionNode(
                                new IdentifierExpressionNode("arr"),
                                new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                            new UnaryExpressionNode(
                                UnaryOperatorType.Minus,
                                new ArrayIndexExpressionNode(
                                    new IdentifierExpressionNode("arr"),
                                    new LiteralExpressionNode("0", LiteralType.IntegerLiteral)))),
                        new ArrayAssignmentStatementNode(
                            new ArrayIndexExpressionNode(
                                new IdentifierExpressionNode("arr"),
                                new IdentifierExpressionNode("idx")),
                            new BinaryExpressionNode(
                                new BinaryExpressionNode(
                                    new UnaryExpressionNode(
                                        UnaryOperatorType.Minus,
                                        new ArrayIndexExpressionNode(
                                            new IdentifierExpressionNode("arr"),
                                            new LiteralExpressionNode("0", LiteralType.IntegerLiteral))),
                                    new ArrayIndexExpressionNode(
                                        new IdentifierExpressionNode("arr"),
                                        new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                                    BinaryOperatorType.Addition),
                                new ArrayIndexExpressionNode(
                                    new IdentifierExpressionNode("arr"),
                                    new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
                                BinaryOperatorType.Multiplication)),
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
    public void SecondExampleTest()
    {
        string source = """
                        =/
                        Our first simple program on CZFF 
                        /=
                        func void Main() {
                            var int n = 5;
                            var array<int> arr = new int(n)[];
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
                        """;
        var expectedAst = AstStore.GetAst("SecondExample");
        
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
    public void DifferentParamsAmountFunctionsTest()
    {
        string source = """
                        func void empty() {
                            return;
                        }
                        func int SumOne(int a) {
                            return a;
                        }
                        func int SumTwo(int a, int b) {
                            return a + b;
                        }
                        func int SumThree(int a, int b, int c) {
                            return a + b + c;
                        }
                        func void Main() {
                            empty();
                            var int x = 1;
                            var int y = SumOne(x);
                            var int z = SumTwo(x, y);
                            var int k;
                            k = SumThree(x, y, z);

                            return;
                        }
                        """;

        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>
            {
                new FunctionDeclarationNode(
                    new SimpleTypeNode("void"), 
                    "empty",
                    new FunctionParametersNode(),
                    new BlockNode(new List<StatementNode>()
                    {
                        new ReturnStatementNode(null),
                    })
                ),
                new FunctionDeclarationNode(
                    new SimpleTypeNode("int"), 
                    "SumOne",
                    new FunctionParametersNode(new List<FunctionParametersNode.Variable>()
                    {
                        new FunctionParametersNode.Variable("a", new SimpleTypeNode("int"))
                    }),
                    new BlockNode(new List<StatementNode>()
                    {
                        new ReturnStatementNode(new IdentifierExpressionNode("a")),
                    })
                ),
                new FunctionDeclarationNode(
                    new SimpleTypeNode("int"), 
                    "SumTwo",
                    new FunctionParametersNode(new List<FunctionParametersNode.Variable>()
                    {
                        new FunctionParametersNode.Variable("a", new SimpleTypeNode("int")),
                        new FunctionParametersNode.Variable("b", new SimpleTypeNode("int"))
                    }),
                    new BlockNode(new List<StatementNode>()
                    {
                        new ReturnStatementNode(new BinaryExpressionNode(
                            new IdentifierExpressionNode("a"),
                            new IdentifierExpressionNode("b"),
                            BinaryOperatorType.Addition)),
                    })
                ),
                new FunctionDeclarationNode(
                    new SimpleTypeNode("int"), 
                    "SumThree",
                    new FunctionParametersNode(new List<FunctionParametersNode.Variable>()
                    {
                        new FunctionParametersNode.Variable("a", new SimpleTypeNode("int")),
                        new FunctionParametersNode.Variable("b", new SimpleTypeNode("int")),
                        new FunctionParametersNode.Variable("c", new SimpleTypeNode("int")),
                    }),
                    new BlockNode(new List<StatementNode>()
                    {
                        new ReturnStatementNode(new BinaryExpressionNode(
                            new BinaryExpressionNode(
                                new IdentifierExpressionNode("a"),
                                new IdentifierExpressionNode("b"),
                                BinaryOperatorType.Addition),
                            new IdentifierExpressionNode("c"),
                            BinaryOperatorType.Addition)),
                    })
                ),
                new FunctionDeclarationNode(
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode(),
                    new BlockNode(new List<StatementNode>()
                    {
                        new ExpressionStatementNode(
                            new FunctionCallExpressionNode("empty", new List<ExpressionNode>())),
                        new VariableDeclarationNode(
                            new SimpleTypeNode("int"),
                            "x",
                            new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                        new VariableDeclarationNode(
                            new SimpleTypeNode("int"),
                            "y",
                            new FunctionCallExpressionNode("SumOne", new List<ExpressionNode>(){ new IdentifierExpressionNode("x") })),
                        new VariableDeclarationNode(
                            new SimpleTypeNode("int"),
                            "z",
                            new FunctionCallExpressionNode("SumTwo", new List<ExpressionNode>(){
                                new IdentifierExpressionNode("x"),
                                new IdentifierExpressionNode("y")
                            })),
                        new VariableDeclarationNode(
                            new SimpleTypeNode("int"),
                            "k"),
                        new IdentifierAssignmentStatementNode(
                            new IdentifierExpressionNode("k"),
                            new FunctionCallExpressionNode("SumThree", new List<ExpressionNode>(){
                                new IdentifierExpressionNode("x"),
                                new IdentifierExpressionNode("y"),
                                new IdentifierExpressionNode("z")
                            })),
                        new ReturnStatementNode(null),
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
    public void ThirdExampleTest()
    {
        string source = """
                        =/
                        Our third simple program on CZFF 
                        /=
                        func int Sum(int a, int b) {
                            return a + b;
                        }

                        func void Main() {
                            var int x = 1;
                            var int y = 2;
                            var int z = Sum(x, y);
                            
                            print z;

                            return;
                        }
                        """;

        var expectedAst = AstStore.GetAst("ThirdExample");
        
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
    public void LogicalOperationsTest()
    {
        // (((10 % 3) - 1) > (5 / 2)) || ((!flag) && (true != flag));
        string source = """
                        func void Main() {
                            var bool flag = false;
                            var bool flag2 = 10 % 3 - 1 > 5 / 2 || !flag && true != flag;
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
                            new SimpleTypeNode("bool"),
                            "flag",
                            new LiteralExpressionNode("false", LiteralType.BooleanLiteral)),
                        new VariableDeclarationNode(
                            new SimpleTypeNode("bool"),
                            "flag2",
                            new BinaryExpressionNode(
                                new BinaryExpressionNode(
                                    new BinaryExpressionNode(
                                        new BinaryExpressionNode(
                                            new LiteralExpressionNode("10", LiteralType.IntegerLiteral),
                                            new LiteralExpressionNode("3", LiteralType.IntegerLiteral),
                                            BinaryOperatorType.Modulus),
                                        new LiteralExpressionNode("1", LiteralType.IntegerLiteral),
                                        BinaryOperatorType.Subtraction),
                                    new BinaryExpressionNode(
                                        new LiteralExpressionNode("5", LiteralType.IntegerLiteral),
                                        new LiteralExpressionNode("2", LiteralType.IntegerLiteral),
                                        BinaryOperatorType.Division),
                                    BinaryOperatorType.Greater),
                            new BinaryExpressionNode(
                                new UnaryExpressionNode(
                                    UnaryOperatorType.Negation,
                                    new IdentifierExpressionNode("flag")),
                                new BinaryExpressionNode(
                                    new LiteralExpressionNode("true", LiteralType.BooleanLiteral),
                                    new IdentifierExpressionNode("flag"),
                                    BinaryOperatorType.NotEqual),
                                BinaryOperatorType.LogicalAnd),
                            BinaryOperatorType.LogicalOr)),
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
    public void WhileTest()
    {
        string source = """
                        func void Main() {
                            var int n = 2;
                            while (n > 0) {
                                n = n - 1;
                            }
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
                            new SimpleTypeNode("int"),
                            "n",
                            new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                        new WhileStatementNode(
                            new BinaryExpressionNode(
                                new IdentifierExpressionNode("n"),
                                new LiteralExpressionNode("0", LiteralType.IntegerLiteral),
                                BinaryOperatorType.Greater),
                            new BlockNode(new List<StatementNode>()
                            {
                                new IdentifierAssignmentStatementNode(
                                    new  IdentifierExpressionNode("n"),
                                    new BinaryExpressionNode(
                                        new IdentifierExpressionNode("n"),
                                        new  LiteralExpressionNode("1", LiteralType.IntegerLiteral),
                                        BinaryOperatorType.Subtraction))
                            })),
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
    public void FourthExampleTest()
    {
        string source = """
                        =/
                        Our fourth simple program on CZFF 
                        /=
                        func void Main() {
                            var int x = 0;
                            for (var int i = 0; i < 5; i = i + 1) {
                                x = x + i;
                            }
                        
                            if (i < 10) {
                                print 1;
                            } else {
                                print 2;
                            }
                            return;
                        }
                        """;

        var expectedAst = AstStore.GetAst("FourthExample");
        
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