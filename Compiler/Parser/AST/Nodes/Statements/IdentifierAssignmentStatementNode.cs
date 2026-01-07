using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class IdentifierAssignmentStatementNode : StatementNode
{
    // мб потом конкретизировать, что тут Identifier или ArrayAccess или MemberAccess
    // либо же на этапе какой-нибудь проверки, надо подумать
    public IdentifierExpressionNode Left { get; }
    
    public ExpressionNode Right { get; }

    public IdentifierAssignmentStatementNode(IdentifierExpressionNode left, ExpressionNode right)
    {
        Left = left;
        Right = right;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}