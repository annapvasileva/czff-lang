using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class PrintStatementNode : StatementNode
{
    public ExpressionNode Expression { get; set; }

    public PrintStatementNode(ExpressionNode expression)
    {
        Expression = expression;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}