namespace Compiler.Parser.AST.Nodes;

public enum BinaryOperatorType
{
    Addition,
    Subtraction,
    Multiplication,
    Division,
    Modulus,

    Equal,
    NotEqual,
    Less,
    LessOrEqual,
    Greater,
    GreaterOrEqual,

    LogicalAnd,
    LogicalOr,
}