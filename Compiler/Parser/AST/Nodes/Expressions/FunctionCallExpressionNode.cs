namespace Compiler.Parser.AST.Nodes.Expressions;

public class FunctionCallExpressionNode : ExpressionNode
{
    public string Name { get; }
    public IList<ExpressionNode> Arguments { get; }

    public FunctionCallExpressionNode(string name, IList<ExpressionNode> arguments)
    {
        Name = name;
        Arguments = arguments;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}