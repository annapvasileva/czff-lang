using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class ReturnStatementNode : StatementNode
{
    public ExpressionNode Expression { get; set; }

    public ReturnStatementNode(ExpressionNode expression)
    {
        Expression = expression;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitReturnStatementNode(this);
    }
}