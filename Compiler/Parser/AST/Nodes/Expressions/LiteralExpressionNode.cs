namespace Compiler.Parser.AST.Nodes.Expressions;

public class LiteralExpressionNode : ExpressionNode
{
    public string Value { get; }
    
    public LiteralType Type { get; }

    public LiteralExpressionNode(string value, LiteralType type)
    {
        Value = value;
        Type = type;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}