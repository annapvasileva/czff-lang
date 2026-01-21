namespace Compiler.Parser.AST.Nodes.Expressions;

public class BinaryExpressionNode : ExpressionNode
{
    public ExpressionNode LeftExpression { get; set; }
    
    public ExpressionNode RightExpression { get; set; }
    
    public BinaryOperatorType BinaryOperatorType { get; }

    public BinaryExpressionNode(ExpressionNode leftExpression, ExpressionNode rightExpression,
        BinaryOperatorType binaryOperatorType)
    {
        LeftExpression = leftExpression;
        RightExpression = rightExpression;
        BinaryOperatorType = binaryOperatorType;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}