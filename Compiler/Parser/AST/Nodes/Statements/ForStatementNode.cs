using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class ForStatementNode : StatementNode
{
    public VariableDeclarationNode Init { get; }
    public ExpressionNode Condition { get; }
    public ExpressionNode Post { get; }

    public ForStatementNode(VariableDeclarationNode init, ExpressionNode condition, ExpressionNode post)
    {
        Init = init;
        Condition = condition;
        Post = post;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitForStatementNode(this);
    }
}