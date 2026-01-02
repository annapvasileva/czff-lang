using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class IfStatementNode : StatementNode
{
    public ExpressionNode Condition { get; }
    public BlockNode IfBlock { get; }
    public IList<ElifStatementNode> Elifs { get; }
    public BlockNode? ElseBlock { get; }

    public IfStatementNode(ExpressionNode condition, BlockNode ifBlock, IList<ElifStatementNode> elifs,
        BlockNode? elseBlock)
    {
        Condition = condition;
        IfBlock = ifBlock;
        Elifs = elifs;
        ElseBlock = elseBlock;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitIfStatementNode(this);
    }
}