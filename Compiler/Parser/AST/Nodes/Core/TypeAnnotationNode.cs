using System.Text.Json.Serialization;
using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Core;

[JsonDerivedType(typeof(SimpleTypeNode), typeDiscriminator: "simple")]
[JsonDerivedType(typeof(ArrayTypeNode), typeDiscriminator: "array")]
public abstract class TypeAnnotationNode : AstNode
{
    public abstract string GetName { get; }
}