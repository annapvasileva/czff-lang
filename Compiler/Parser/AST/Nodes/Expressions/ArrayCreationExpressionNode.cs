namespace Compiler.Parser.AST.Nodes.Expressions;

public class ArrayCreationExpressionNode : ExpressionNode
{
    public ExpressionNode Size { get; }

    public ArrayCreationExpressionNode(ExpressionNode size)
    {
        Size = size;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}