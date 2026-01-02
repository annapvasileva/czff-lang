namespace Compiler.Parser.AST.Nodes.Expressions;

public class BinaryExpressionNode : ExpressionNode
{
    public ExpressionNode LeftExpression { get; }
    
    public ExpressionNode RightExpression { get; }
    
    public BinaryOperator BinaryOperator { get; }

    public BinaryExpressionNode(ExpressionNode leftExpression, ExpressionNode rightExpression,
        BinaryOperator binaryOperator)
    {
        LeftExpression = leftExpression;
        RightExpression = rightExpression;
        BinaryOperator = binaryOperator;
    }
    
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitBinaryExpressionNode(this);
    }
}