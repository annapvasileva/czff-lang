using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class ExpressionStatementNode : StatementNode
{
    public ExpressionNode Expression { get; set; }

    public ExpressionStatementNode(ExpressionNode expression)
    {
        Expression = expression;
    }

    public override void Accept(IVisitor visitor)
    {
        visitor.VisitExpressionStatementNode(this);
    }
}