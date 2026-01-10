namespace Compiler.Parser.AST.Nodes.Core;

public class SimpleTypeNode : TypeAnnotationNode
{
    public string Name { get; }
    // ссылка на class symbol?
    public SimpleTypeNode(string name)
    {
        switch (name)
        {
            case "int":
                Name = "I";
                break;
            default:
                Name = name;
                break;
        }
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override string GetName => Name + ";";
}