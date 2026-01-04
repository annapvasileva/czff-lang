using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Parser.AST.Nodes;

public class BlockNode : AstNode
{
    public IList<StatementNode> Statements { get; }

    public BlockNode(IList<StatementNode> statements)
    {
        Statements = statements;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}