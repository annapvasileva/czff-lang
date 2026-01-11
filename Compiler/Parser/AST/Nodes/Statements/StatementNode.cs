using System.Text.Json.Serialization;

namespace Compiler.Parser.AST.Nodes.Statements;

[JsonDerivedType(typeof(VariableDeclarationNode), typeDiscriminator: "variable")]
[JsonDerivedType(typeof(PrintStatementNode), typeDiscriminator: "print")]
[JsonDerivedType(typeof(ReturnStatementNode), typeDiscriminator: "return")]
[JsonDerivedType(typeof(ArrayAssignmentStatementNode), typeDiscriminator: "arrayAssigment")]
[JsonDerivedType(typeof(IdentifierAssignmentStatementNode), typeDiscriminator: "identifierAssigment")]
[JsonDerivedType(typeof(ExpressionStatementNode), typeDiscriminator: "expressionStatement")]
[JsonDerivedType(typeof(IfStatementNode), typeDiscriminator: "if")]
[JsonDerivedType(typeof(ForStatementNode), typeDiscriminator: "for")]
[JsonDerivedType(typeof(WhileStatementNode), typeDiscriminator: "while")]
[JsonDerivedType(typeof(BreakStatementNode), typeDiscriminator: "break")]
[JsonDerivedType(typeof(ContinueStatementNode), typeDiscriminator: "continue")]
public abstract class StatementNode : AstNode { }