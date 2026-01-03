namespace Compiler.Parser.AST.Nodes.Statements;

public class BreakStatementNode : StatementNode
{
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}