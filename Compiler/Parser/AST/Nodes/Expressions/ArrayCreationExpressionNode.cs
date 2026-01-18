using Compiler.Parser.AST.Nodes.Core;

namespace Compiler.Parser.AST.Nodes.Expressions;

public class ArrayCreationExpressionNode : ExpressionNode
{
    public TypeAnnotationNode ElementType { get; set; }
    public ExpressionNode Size { get; set; }

    public ArrayCreationExpressionNode(TypeAnnotationNode elementType, ExpressionNode size)
    {
        ElementType = elementType;
        Size = size;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}