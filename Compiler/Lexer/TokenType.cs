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
    Break,
    Continue,

    Integer,
    Integer64,
    Integer128,
    Void,
    Array,
    Bool,

    IntegerLiteral,
    Integer64Literal,
    Integer128Literal,
    StringLiteral,
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
    QuotationMark,
    
    Eof,
}