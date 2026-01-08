using Compiler.Lexer;

namespace Compiler.Tests.LexerTests;

public class LexerTests
{
    [Theory]
    [ClassData(typeof(OneLexemeTestData))]
    public void OneLexemeTest(string source, Token expectedToken)
    {
        var expected = new List<Token>()
        {
            expectedToken,
            new (TokenType.Eof, "\0", 2, 1),
        };
        
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();

        Assert.Equal(expected.Count, tokens.Count);
        Assert.Equal(expected, tokens);
    }

    [Theory]
    [ClassData(typeof(WhitespacesCommentsTestsData))]
    public void WhitespacesCommentsTest(string source, IList<Token> expected)
    {
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();

        Assert.Equal(expected.Count, tokens.Count);
        Assert.Equal(expected, tokens);
    }

    [Fact]
    public void UnclosedCommentTest()
    {
        string source = """
                        int ababb
                        =/ some
                        thing
                        ...
                        """;
        var lexer = new CompilerLexer(source);
        LexerException exception = Assert.Throws<LexerException>(() => lexer.GetTokens().ToList());
        Assert.Contains(exception.Message, $"Unclosed comment at line 2, column 1");
    }

    [Fact]
    public void Test1()
    {
        var expected = new List<Token>()
        {
            new (TokenType.Var, "var", 1, 1),
            new (TokenType.Integer, "int", 1, 5),
            new (TokenType.Identifier, "a", 1, 9),
            new (TokenType.Assign, "=", 1, 11),
            new (TokenType.IntegerLiteral, "50", 1, 13),
            new (TokenType.Semicolon, ";", 1, 15),
            new (TokenType.Eof, "\0", 2, 1),
        };
        var lexer = new CompilerLexer("var int a = 50;");
        var tokens = lexer.GetTokens().ToList();
        
        Assert.Equal(expected.Count, tokens.Count);
        Assert.Equal(expected, tokens);
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
        var expected = new List<Token>()
        {
            new (TokenType.Func, "func", 4, 1),
            new (TokenType.Void, "void", 4, 6),
            new (TokenType.Identifier, "Main", 4, 11),
            new (TokenType.LeftRoundBracket, "(", 4, 15),
            new (TokenType.RightRoundBracket, ")", 4, 16),
            new (TokenType.LeftCurlyBracket, "{", 4, 18),
            
            new (TokenType.Var, "var", 5, 5),
            new (TokenType.Integer, "int", 5, 9),
            new (TokenType.Identifier, "a", 5, 13),
            new (TokenType.Assign, "=", 5, 15),
            new (TokenType.IntegerLiteral, "2", 5, 17),
            new (TokenType.Semicolon, ";", 5, 18),
            
            new (TokenType.Var, "var", 6, 5),
            new (TokenType.Integer, "int", 6, 9),
            new (TokenType.Identifier, "b", 6, 13),
            new (TokenType.Assign, "=", 6, 15),
            new (TokenType.IntegerLiteral, "3", 6, 17),
            new (TokenType.Semicolon, ";", 6, 18),
            
            new (TokenType.Var, "var", 7, 5),
            new (TokenType.Integer, "int", 7, 9),
            new (TokenType.Identifier, "res", 7, 13),
            new (TokenType.Assign, "=", 7, 17),
            new (TokenType.Identifier, "a", 7, 19),
            new (TokenType.Plus, "+", 7, 21),
            new (TokenType.Identifier, "b", 7, 23),
            new (TokenType.Semicolon, ";", 7, 24),
            
            new (TokenType.Print, "print", 8, 5),
            new (TokenType.Identifier, "res", 8, 11),
            new (TokenType.Semicolon, ";", 8, 14),
            
            new (TokenType.Return, "return", 9, 5),
            new (TokenType.Semicolon, ";", 9, 11),
            
            new (TokenType.RightCurlyBracket, "}", 10, 1),
            
            new (TokenType.Eof, "\0", 11, 1),
        };
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        
        Assert.Equal(expected.Count, tokens.Count);
        Assert.Equal(expected, tokens);
    }

    [Fact]
    public void ArrayExampleTest()
    {
        string source = """
                        =/
                        Our second simple program on CZFF 
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
        var expected = new List<Token>()
        {
            new (TokenType.Func, "func", 4, 1),
            new (TokenType.Void, "void", 4, 6),
            new (TokenType.Identifier, "Main", 4, 11),
            new (TokenType.LeftRoundBracket, "(", 4, 15),
            new (TokenType.RightRoundBracket, ")", 4, 16),
            new (TokenType.LeftCurlyBracket, "{", 4, 18),

            new (TokenType.Var, "var", 5, 5),
            new (TokenType.Integer, "int", 5, 9),
            new (TokenType.Identifier, "n", 5, 13),
            new (TokenType.Assign, "=", 5, 15),
            new (TokenType.IntegerLiteral, "5", 5, 17),
            new (TokenType.Semicolon, ";", 5, 18),

            new (TokenType.Var, "var", 6, 5),
            new (TokenType.Array, "array", 6, 9),
            new (TokenType.Less, "<", 6, 14),
            new (TokenType.Integer, "int", 6, 15),
            new (TokenType.Greater, ">", 6, 18),
            new (TokenType.Identifier, "arr", 6, 20),
            new (TokenType.Assign, "=", 6, 24),
            new (TokenType.New, "new", 6, 26),
            new (TokenType.Integer, "int", 6, 30),
            new (TokenType.LeftRoundBracket, "(", 6, 33),
            new (TokenType.Identifier, "n", 6, 34),
            new (TokenType.RightRoundBracket, ")", 6, 35),
            new (TokenType.LeftSquareBracket, "[", 6, 36),
            new (TokenType.RightSquareBracket, "]", 6, 37),
            new (TokenType.Semicolon, ";", 6, 38),

            new (TokenType.Identifier, "arr", 7, 5),
            new (TokenType.LeftSquareBracket, "[", 7, 8),
            new (TokenType.IntegerLiteral, "0", 7, 9),
            new (TokenType.RightSquareBracket, "]", 7, 10),
            new (TokenType.Assign, "=", 7, 12),
            new (TokenType.IntegerLiteral, "-1", 7, 14),
            new (TokenType.Semicolon, ";", 7, 16),

            new (TokenType.Identifier, "arr", 8, 5),
            new (TokenType.LeftSquareBracket, "[", 8, 8),
            new (TokenType.IntegerLiteral, "1", 8, 9),
            new (TokenType.RightSquareBracket, "]", 8, 10),
            new (TokenType.Assign, "=", 8, 12),
            new (TokenType.IntegerLiteral, "2", 8, 14),
            new (TokenType.Semicolon, ";", 8, 15),

            new (TokenType.Identifier, "arr", 9, 5),
            new (TokenType.LeftSquareBracket, "[", 9, 8),
            new (TokenType.IntegerLiteral, "2", 9, 9),
            new (TokenType.RightSquareBracket, "]", 9, 10),
            new (TokenType.Assign, "=", 9, 12),
            new (TokenType.Identifier, "arr", 9, 14),
            new (TokenType.LeftSquareBracket, "[", 9, 17),
            new (TokenType.IntegerLiteral, "0", 9, 18),
            new (TokenType.RightSquareBracket, "]", 9, 19),
            new (TokenType.Plus, "+", 9, 21),
            new (TokenType.Identifier, "arr", 9, 23),
            new (TokenType.LeftSquareBracket, "[", 9, 26),
            new (TokenType.IntegerLiteral, "1", 9, 27),
            new (TokenType.RightSquareBracket, "]", 9, 28),
            new (TokenType.Semicolon, ";", 9, 29),

            new (TokenType.Identifier, "arr", 10, 5),
            new (TokenType.LeftSquareBracket, "[", 10, 8),
            new (TokenType.IntegerLiteral, "3", 10, 9),
            new (TokenType.RightSquareBracket, "]", 10, 10),
            new (TokenType.Assign, "=", 10, 12),
            new (TokenType.Minus, "-", 10, 14),
            new (TokenType.LeftRoundBracket, "(", 10, 15),
            new (TokenType.Identifier, "arr", 10, 16),
            new (TokenType.LeftSquareBracket, "[", 10, 19),
            new (TokenType.IntegerLiteral, "0", 10, 20),
            new (TokenType.RightSquareBracket, "]", 10, 21),
            new (TokenType.Multiply, "*", 10, 23),
            new (TokenType.Identifier, "arr", 10, 25),
            new (TokenType.LeftSquareBracket, "[", 10, 28),
            new (TokenType.IntegerLiteral, "1", 10, 29),
            new (TokenType.RightSquareBracket, "]", 10, 30),
            new (TokenType.RightRoundBracket, ")", 10, 31),
            new (TokenType.Semicolon, ";", 10, 32),

            new (TokenType.Identifier, "arr", 11, 5),
            new (TokenType.LeftSquareBracket, "[", 11, 8),
            new (TokenType.IntegerLiteral, "4", 11, 9),
            new (TokenType.RightSquareBracket, "]", 11, 10),
            new (TokenType.Assign, "=", 11, 12),
            new (TokenType.Identifier, "arr", 11, 14),
            new (TokenType.LeftSquareBracket, "[", 11, 17),
            new (TokenType.IntegerLiteral, "0", 11, 18),
            new (TokenType.RightSquareBracket, "]", 11, 19),
            new (TokenType.Multiply, "*", 11, 21),
            new (TokenType.LeftRoundBracket, "(", 11, 23),
            new (TokenType.Identifier, "arr", 11, 24),
            new (TokenType.LeftSquareBracket, "[", 11, 27),
            new (TokenType.IntegerLiteral,"1", 11, 28),
            new (TokenType.RightSquareBracket, "]", 11, 29),
            new (TokenType.Plus, "+", 11, 31),
            new (TokenType.Identifier, "arr", 11, 33),
            new (TokenType.LeftSquareBracket, "[", 11, 36),
            new (TokenType.IntegerLiteral,"2", 11, 37),
            new (TokenType.RightSquareBracket, "]", 11, 38),
            new (TokenType.RightRoundBracket, ")", 11, 39),
            new (TokenType.Plus, "+", 11, 41),
            new (TokenType.Identifier, "arr", 11, 43),
            new (TokenType.LeftSquareBracket, "[", 11, 46),
            new (TokenType.IntegerLiteral,"3", 11, 47),
            new (TokenType.RightSquareBracket, "]", 11, 48),
            new (TokenType.Semicolon, ";", 11, 49),

            new (TokenType.Print, "print", 13, 5),
            new (TokenType.Identifier, "arr", 13, 11),
            new (TokenType.LeftSquareBracket, "[", 13, 14),
            new (TokenType.IntegerLiteral,"0", 13, 15),
            new (TokenType.RightSquareBracket, "]", 13, 16),
            new (TokenType.Semicolon, ";", 13, 17),

            new (TokenType.Print, "print", 14, 5),
            new (TokenType.Identifier, "arr", 14, 11),
            new (TokenType.LeftSquareBracket, "[", 14, 14),
            new (TokenType.IntegerLiteral,"1", 14, 15),
            new (TokenType.RightSquareBracket, "]", 14, 16),
            new (TokenType.Semicolon, ";", 14, 17),

            new (TokenType.Print, "print", 15, 5),
            new (TokenType.Identifier, "arr", 15, 11),
            new (TokenType.LeftSquareBracket, "[", 15, 14),
            new (TokenType.IntegerLiteral,"2", 15, 15),
            new (TokenType.RightSquareBracket, "]", 15, 16),
            new (TokenType.Semicolon, ";", 15, 17),

            new (TokenType.Print, "print", 16, 5),
            new (TokenType.Identifier, "arr", 16, 11),
            new (TokenType.LeftSquareBracket, "[", 16, 14),
            new (TokenType.IntegerLiteral,"3", 16, 15),
            new (TokenType.RightSquareBracket, "]", 16, 16),
            new (TokenType.Semicolon, ";", 16, 17),

            new (TokenType.Print, "print", 17, 5),
            new (TokenType.Identifier, "arr", 17, 11),
            new (TokenType.LeftSquareBracket, "[", 17, 14),
            new (TokenType.IntegerLiteral,"4", 17, 15),
            new (TokenType.RightSquareBracket, "]", 17, 16),
            new (TokenType.Semicolon, ";", 17, 17),

            new (TokenType.Return, "return", 19, 5),
            new (TokenType.Semicolon, ";", 19, 11),

            new (TokenType.RightCurlyBracket, "}", 20, 1),

            new (TokenType.Eof, "\0", 21, 1),
        };
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        
        Assert.Equal(expected.Count, tokens.Count);
        Assert.Equal(expected, tokens);
    }
}