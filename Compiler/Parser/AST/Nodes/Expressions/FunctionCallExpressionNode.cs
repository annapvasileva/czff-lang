namespace Compiler.Parser.AST.Nodes.Expressions;

public class FunctionCallExpressionNode : ExpressionNode
{
    public AstNode Callee { get; }
    public IList<ExpressionNode> Arguments { get; }

    public FunctionCallExpressionNode(AstNode callee, IList<ExpressionNode> arguments)
    {
        Callee = callee;
        Arguments = arguments;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}