using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class ElifStatementNode : StatementNode
{
    public ExpressionNode Condition { get; }
    public BlockNode Block { get; }

    public ElifStatementNode(ExpressionNode condition, BlockNode block)
    {
        Condition = condition;
        Block = block;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}