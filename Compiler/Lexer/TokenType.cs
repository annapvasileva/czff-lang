namespace Compiler.Lexer;

public enum TokenType
{
    Identifier,
    Var,
    Func,
    Print,
    Return,
    New,
    If,
    Else,
    For,
    While,

    Integer,
    Void,
    Array,
    Bool,

    IntegerLiteral,
    BoolLiteral,

    Plus,
    Minus,
    Multiply,
    Divide,
    Modulo,

    LogicalOr,
    LogicalAnd,
    LogicalNegation,

    Assign,

    Less,
    Greater,
    LessEqual,
    GreaterEqual,
    Equal,
    NotEqual,

    Comma,              // ,
    Semicolon,          // ;
    LeftRoundBracket,   // (
    RightRoundBracket,  // )
    LeftCurlyBracket,   // {
    RightCurlyBracket,  // }
    LeftSquareBracket,  // [
    RightSquareBracket, // ]
    
    Eof,
}