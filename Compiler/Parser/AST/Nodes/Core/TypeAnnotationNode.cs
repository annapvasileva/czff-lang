using System.Text.Json.Serialization;
using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Core;

[JsonDerivedType(typeof(SimpleTypeNode), typeDiscriminator: "simple")]
public abstract class TypeAnnotationNode : AstNode
{
    public abstract string GetName { get; }
}