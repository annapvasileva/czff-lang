using System.Collections;
using Compiler.Lexer;

namespace Compiler.Tests.LexerTests;

public class OneLexemeTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] 
        { 
            "ababb", 
            new Token(TokenType.Identifier, "ababb", 1, 1) 
        };

        yield return new object[] 
        { 
            "var",
            new Token(TokenType.Var, "var", 1, 1) 
        };

        yield return new object[] 
        { 
            "func", 
            new Token(TokenType.Func, "func", 1, 1) 
        };

        yield return new object[] 
        { 
            "print", 
            new Token(TokenType.Print, "print", 1, 1) 
        };

        yield return new object[] 
        { 
            "int", 
            new Token(TokenType.Integer, "int", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "void", 
            new Token(TokenType.Void, "void", 1, 1) 
        };

        yield return new object[] 
        { 
            "12389128432241", 
            new Token(TokenType.IntegerLiteral, "12389128432241", 1, 1) 
        };

        yield return new object[] 
        { 
            "+", 
            new Token(TokenType.Plus, "+", 1, 1) 
        };

        yield return new object[] 
        { 
            "=", 
            new Token(TokenType.Assign, "=", 1, 1) 
        };

        yield return new object[] 
        { 
            ";", 
            new Token(TokenType.Semicolon, ";", 1, 1) 
        };

        yield return new object[] 
        { 
            "(", 
            new Token(TokenType.LeftRoundBracket, "(", 1, 1) 
        };

        yield return new object[] 
        { 
            ")", 
            new Token(TokenType.RightRoundBracket, ")", 1, 1) 
        };

        yield return new object[] 
        { 
            "{", 
            new Token(TokenType.LeftCurlyBracket, "{", 1, 1) 
        };

        yield return new object[] 
        { 
            "}", 
            new Token(TokenType.RightCurlyBracket, "}", 1, 1) 
        };

        yield return new object[] 
        { 
            "return", 
            new Token(TokenType.Return, "return", 1, 1) 
        };

        yield return new object[] 
        { 
            "-123", 
            new Token(TokenType.IntegerLiteral, "-123", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "new", 
            new Token(TokenType.New, "new", 1, 1) 
        };

        yield return new object[] 
        { 
            "array", 
            new Token(TokenType.Array, "array", 1, 1) 
        };

        yield return new object[] 
        { 
            "*", 
            new Token(TokenType.Multiply, "*", 1, 1) 
        };

        yield return new object[] 
        { 
            "<", 
            new Token(TokenType.Less, "<", 1, 1) 
        };

        yield return new object[] 
        { 
            ">", 
            new Token(TokenType.Greater, ">", 1, 1) 
        };

        yield return new object[] 
        { 
            "[", 
            new Token(TokenType.LeftSquareBracket, "[", 1, 1) 
        };

        yield return new object[] 
        { 
            "]", 
            new Token(TokenType.RightSquareBracket, "]", 1, 1) 
        };

        yield return new object[] 
        { 
            "-", 
            new Token(TokenType.Minus, "-", 1, 1) 
        };
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
