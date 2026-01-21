namespace Compiler.Parser.AST.Nodes.Expressions;

public class MemberAccessNode : ExpressionNode
{
    public ExpressionNode Expression { get; }
    public string Member { get; }

    public MemberAccessNode(ExpressionNode expression, string member)
    {
        Expression = expression;
        Member = member;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}