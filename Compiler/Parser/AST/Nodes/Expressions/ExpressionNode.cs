using System.Text.Json.Serialization;
using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Parser.AST.Nodes.Expressions;

[JsonDerivedType(typeof(BinaryExpressionNode), typeDiscriminator: "binary")]
[JsonDerivedType(typeof(IdentifierExpressionNode), typeDiscriminator: "identifier")]
[JsonDerivedType(typeof(LiteralExpressionNode), typeDiscriminator: "literal")]
public abstract class ExpressionNode : AstNode { }