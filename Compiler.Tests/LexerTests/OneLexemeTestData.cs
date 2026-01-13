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
            "123L", 
            new Token(TokenType.Integer64Literal, "123", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "-9123L", 
            new Token(TokenType.Integer64Literal, "-9123", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "12389128432241L", 
            new Token(TokenType.Integer64Literal, "12389128432241", 1, 1) 
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
        
        yield return new object[] 
        { 
            ",",
            new Token(TokenType.Comma, ",", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "bool",
            new Token(TokenType.Bool, "bool", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "true",
            new Token(TokenType.BoolLiteral, "true", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "false",
            new Token(TokenType.BoolLiteral, "false", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "/",
            new Token(TokenType.Divide, "/", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "%",
            new Token(TokenType.Modulo, "%", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "!",
            new Token(TokenType.LogicalNegation, "!", 1, 1) 
        };

        yield return new object[] 
        { 
            "&&",
            new Token(TokenType.LogicalAnd, "&&", 1, 1) 
        };

        yield return new object[] 
        { 
            "||",
            new Token(TokenType.LogicalOr, "||", 1, 1) 
        };

        yield return new object[] 
        { 
            "<=",
            new Token(TokenType.LessEqual, "<=", 1, 1) 
        };

        yield return new object[] 
        { 
            ">=",
            new Token(TokenType.GreaterEqual, ">=", 1, 1) 
        };

        yield return new object[] 
        { 
            "==",
            new Token(TokenType.Equal, "==", 1, 1) 
        };

        yield return new object[] 
        { 
            "!=",
            new Token(TokenType.NotEqual, "!=", 1, 1) 
        };

        yield return new object[] 
        { 
            "if",
            new Token(TokenType.If, "if", 1, 1) 
        };

        yield return new object[] 
        { 
            "else",
            new Token(TokenType.Else, "else", 1, 1) 
        };

        yield return new object[] 
        { 
            "for",
            new Token(TokenType.For, "for", 1, 1) 
        };

        yield return new object[] 
        { 
            "while",
            new Token(TokenType.While, "while", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "break",
            new Token(TokenType.Break, "break", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "continue",
            new Token(TokenType.Continue, "continue", 1, 1) 
        };

        yield return new object[] 
        { 
            "int64",
            new Token(TokenType.Integer64, "int64", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "int128",
            new Token(TokenType.Integer128, "int128", 1, 1) 
        };
        
        yield return new object[] 
        { 
            "\"abacaba;hello  fskdjnfd\"",
            new Token(TokenType.StringLiteral, "abacaba;hello  fskdjnfd", 1, 1) 
        };
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
