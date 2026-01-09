using System.Text.Json.Serialization;

namespace Compiler.Parser.AST.Nodes.Statements;

[JsonDerivedType(typeof(VariableDeclarationNode), typeDiscriminator: "variable")]
[JsonDerivedType(typeof(PrintStatementNode), typeDiscriminator: "print")]
[JsonDerivedType(typeof(ReturnStatementNode), typeDiscriminator: "return")]
[JsonDerivedType(typeof(ArrayAssignmentStatementNode), typeDiscriminator: "arrayAssigment")]
[JsonDerivedType(typeof(IdentifierAssignmentStatementNode), typeDiscriminator: "identifierAssigment")]
public abstract class StatementNode : AstNode { }