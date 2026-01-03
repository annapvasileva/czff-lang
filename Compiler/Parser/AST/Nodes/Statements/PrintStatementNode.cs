using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class PrintStatementNode : StatementNode
{
    public IList<ExpressionNode> Expressions { get; }

    public PrintStatementNode(IList<ExpressionNode> expressions)
    {
        Expressions = expressions;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}