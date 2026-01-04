namespace Compiler.Parser.AST.Nodes.Core;

public class SimpleTypeNode : TypeAnnotationNode
{
    public string Name { get; }
    // ссылка на class symbol?
    public SimpleTypeNode(string name)
    {
        Name = name;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}