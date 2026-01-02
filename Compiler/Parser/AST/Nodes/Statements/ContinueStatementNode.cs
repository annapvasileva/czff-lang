namespace Compiler.Parser.AST.Nodes.Statements;

public class ContinueStatementNode : StatementNode
{
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitContinueStatementNode(this);
    }
}