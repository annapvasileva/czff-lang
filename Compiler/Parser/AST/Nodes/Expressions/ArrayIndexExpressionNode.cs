namespace Compiler.Parser.AST.Nodes.Expressions;

public class ArrayIndexExpressionNode : ExpressionNode
{
    public ExpressionNode Array { get; }
    public ExpressionNode Index { get; }

    public ArrayIndexExpressionNode(ExpressionNode array, ExpressionNode index)
    {
        Array = array;
        Index = index;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitArrayIndexExpressionNode(this);
    }
}