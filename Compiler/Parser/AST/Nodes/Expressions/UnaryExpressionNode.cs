namespace Compiler.Parser.AST.Nodes.Expressions;

public class UnaryExpressionNode : ExpressionNode
{
    public UnaryOperatorType UnaryOperatorType { get; }
    public ExpressionNode Expression { get; }

    public UnaryExpressionNode(UnaryOperatorType unaryOperatorType, ExpressionNode expression)
    {
        UnaryOperatorType = unaryOperatorType;
        Expression = expression;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}