namespace Compiler.Parser.AST.Nodes.Statements;

public class ContinueStatementNode : StatementNode
{
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}