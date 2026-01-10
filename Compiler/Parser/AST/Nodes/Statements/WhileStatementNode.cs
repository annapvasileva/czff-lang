using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class WhileStatementNode : StatementNode
{
    public ExpressionNode Condition { get; }
    public BlockNode Body { get; }

    public WhileStatementNode(ExpressionNode condition, BlockNode body)
    {
        Condition = condition;
        Body = body;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}