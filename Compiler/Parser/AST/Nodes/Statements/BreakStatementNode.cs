namespace Compiler.Parser.AST.Nodes.Statements;

public class BreakStatementNode : StatementNode
{
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitBreakStatementNode(this);
    }
}