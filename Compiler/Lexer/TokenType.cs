namespace Compiler.Lexer;

public enum TokenType
{
    Identifier,
    Var,
    Func,
    Print,
    
    Integer,
    Void,
    
    IntegerLiteral,
    
    Plus,
    Assign,
    
    Semicolon,          // ;
    LeftRoundBracket,   // (
    RightRoundBracket,  // )
    LeftCurlyBracket,   // {
    RightCurlyBracket,  // }
    
    Eof,
}