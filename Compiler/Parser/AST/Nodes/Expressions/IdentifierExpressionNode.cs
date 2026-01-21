namespace Compiler.Parser.AST.Nodes.Expressions;

public class IdentifierExpressionNode : ExpressionNode
{
    public string Name { get; }
    
    public IdentifierExpressionNode(string name) => Name = name;
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}