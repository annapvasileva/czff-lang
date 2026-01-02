namespace Compiler.Lexer;

public enum TokenType
{
    Identifier,
    Var,
    Func,
    Print,
    
    Integer,
    // void
    
    IntegerLiteral,
    
    Plus,
    Assign,
    
    LeftRoundBracket, // (
    RightRoundBracket, // )
    LeftCurlyBracket, // {
    RightCurlyBracket, // }
    
    Eof,
}