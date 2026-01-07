using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class ArrayAssignmentStatementNode : StatementNode
{
    // мб потом конкретизировать, что тут Identifier или ArrayAccess или MemberAccess
    // либо же на этапе какой-нибудь проверки, надо подумать
    public ArrayIndexExpressionNode Left { get; }
    
    public ExpressionNode Right { get; }

    public ArrayAssignmentStatementNode(ArrayIndexExpressionNode left, ExpressionNode right)
    {
        Left = left;
        Right = right;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}