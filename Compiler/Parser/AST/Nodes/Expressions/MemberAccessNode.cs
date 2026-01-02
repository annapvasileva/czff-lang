using Compiler.Parser.AST.Nodes.Core;

namespace Compiler.Parser.AST.Nodes.Expressions;

public class MemberAccessNode : ExpressionNode
{
    public ExpressionNode Expression { get; }
    public IdentifierNode Member { get; }

    public MemberAccessNode(ExpressionNode expression, IdentifierNode member)
    {
        Expression = expression;
        Member = member;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitMemberAccessNode(this);
    }
}