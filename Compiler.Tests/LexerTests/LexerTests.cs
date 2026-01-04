using Compiler.Lexer;

namespace Compiler.Tests;

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
            
            new (TokenType.RightCurlyBracket, "}", 9, 1),
            
            new (TokenType.Eof, "\0", 10, 1),
        };
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        
        Assert.Equal(expected.Count, tokens.Count);
        Assert.Equal(expected, tokens);
    }
}