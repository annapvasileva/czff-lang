using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class ForStatementNode : StatementNode
{
    public VariableDeclarationNode Init { get; }
    public ExpressionNode Condition { get; set; }
    public StatementNode Post { get; }
    public BlockNode Body { get; }

    public ForStatementNode(VariableDeclarationNode init, ExpressionNode condition, StatementNode post, BlockNode body)
    {
        Init = init;
        Condition = condition;
        Post = post;
        Body = body;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}