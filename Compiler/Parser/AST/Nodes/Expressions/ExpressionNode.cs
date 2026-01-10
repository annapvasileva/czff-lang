using System.Text.Json.Serialization;
using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Parser.AST.Nodes.Expressions;

[JsonDerivedType(typeof(BinaryExpressionNode), typeDiscriminator: "binary")]
[JsonDerivedType(typeof(IdentifierExpressionNode), typeDiscriminator: "identifier")]
[JsonDerivedType(typeof(LiteralExpressionNode), typeDiscriminator: "literal")]
[JsonDerivedType(typeof(ArrayCreationExpressionNode), typeDiscriminator: "arrayCreation")]
[JsonDerivedType(typeof(ArrayIndexExpressionNode), typeDiscriminator: "arrayIndex")]
[JsonDerivedType(typeof(UnaryExpressionNode), typeDiscriminator: "unary")]
[JsonDerivedType(typeof(FunctionCallExpressionNode), typeDiscriminator: "functionCall")]
public abstract class ExpressionNode : AstNode { }