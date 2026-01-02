namespace Compiler.Parser.AST.Nodes.Core;

public class LiteralNode : AstNode
{
    public string Value { get; }
    
    public LiteralType Type { get; }

    public LiteralNode(string value, LiteralType type)
    {
        Value = value;
        Type = type;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitLiteralNode(this);
    }
}