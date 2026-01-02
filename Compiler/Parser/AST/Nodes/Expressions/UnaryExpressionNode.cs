namespace Compiler.Parser.AST.Nodes.Expressions;

public class UnaryExpressionNode : ExpressionNode
{
    public UnaryOperator UnaryOperator { get; }
    public ExpressionNode Expression { get; }

    public UnaryExpressionNode(UnaryOperator unaryOperator, ExpressionNode expression)
    {
        UnaryOperator = unaryOperator;
        Expression = expression;
    }

    public override void Accept(IVisitor visitor)
    {
        visitor.VisitUnaryExpressionNode(this);
    }
}