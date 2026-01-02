using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class WhileStatementNode : StatementNode
{
    public ExpressionNode Condition { get; }
    public StatementNode Body { get; }

    public WhileStatementNode(ExpressionNode condition, StatementNode body)
    {
        Condition = condition;
        Body = body;
    }
    
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitWhileStatementNode(this);
    }
}