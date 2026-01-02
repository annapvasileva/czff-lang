using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class AssignmentStatementNode : StatementNode
{
    // мб потом конкретизировать, что тут Identifier или ArrayAccess или MemberAccess
    // либо же на этапе какой-нибудь проверки, надо подумать
    public ExpressionNode Left { get; }
    
    public ExpressionNode Right { get; }

    public AssignmentStatementNode(ExpressionNode left, ExpressionNode right)
    {
        Left = left;
        Right = right;
    }
    
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitAssignmentStatementNode(this);
    }
}