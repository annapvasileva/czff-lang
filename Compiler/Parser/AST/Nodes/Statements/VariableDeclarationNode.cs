using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class VariableDeclarationNode : DeclarationNode
{
    public TypeAnnotationNode Type { get; }
    public string Name { get; }
    public ExpressionNode? Expression { get; set; }
    
    public VariableDeclarationNode(TypeAnnotationNode type, string name)
    {
        Type = type;
        Name = name;
    }

    public VariableDeclarationNode(TypeAnnotationNode type, string name,
        ExpressionNode expression)
    {
        Type = type;
        Name = name;
        Expression = expression;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}