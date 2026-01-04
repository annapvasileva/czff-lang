using System.Text.Json.Serialization;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Parser.AST;

[JsonDerivedType(typeof(ProgramNode), typeDiscriminator: "program")]
[JsonDerivedType(typeof(FunctionDeclarationNode), typeDiscriminator: "function")]
[JsonDerivedType(typeof(VariableDeclarationNode), typeDiscriminator: "variable")]
[JsonDerivedType(typeof(BlockNode), typeDiscriminator: "block")]
public abstract class AstNode
{
    public abstract void Accept(INodeVisitor visitor);
}