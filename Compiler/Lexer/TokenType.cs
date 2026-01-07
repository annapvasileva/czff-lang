namespace Compiler.Lexer;

public enum TokenType
{
    Identifier,
    Var,
    Func,
    Print,
    Return,
    New,

    Integer,
    Void,
    Array,

    IntegerLiteral,

    Plus,
    Minus,
    Multiply,
    Assign,

    Less,
    Greater,

    Semicolon,          // ;
    LeftRoundBracket,   // (
    RightRoundBracket,  // )
    LeftCurlyBracket,   // {
    RightCurlyBracket,  // }
    LeftSquareBracket,  // [
    RightSquareBracket, // ]
    
    Eof,
}