using System.Text.Json.Serialization;

namespace Compiler.Parser.AST.Nodes.Statements;

[JsonDerivedType(typeof(VariableDeclarationNode), typeDiscriminator: "variable")]
[JsonDerivedType(typeof(PrintStatementNode), typeDiscriminator: "print")]
public abstract class StatementNode : AstNode { }